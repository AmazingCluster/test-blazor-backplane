using Blazored.LocalStorage;

using BlazorSignalRBackplaneTest.Data;
using BlazorSignalRBackplaneTest.State.Storage;

using Fluxor;
using Fluxor.Persist.Middleware;
using Fluxor.Persist.Storage;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

using StackExchange.Redis;

namespace BlazorSignalRBackplaneTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();

            builder.Services.AddHttpClient();
            builder.Services.AddLogging();

            ConfigurationOptions redisConfigurationOptions = new ConfigurationOptions
            {
                EndPoints = { { builder.Configuration.GetValue<string>("redis:host"), builder.Configuration.GetValue<int>("redis:port") } },
                Password = builder.Configuration.GetValue<string>("REDIS_PASSWORD"),
                ServiceName = builder.Configuration.GetValue<string>("redis:masterSet"),
                AllowAdmin = builder.Configuration.GetValue<bool>("redis:allowAdmin")
            };

            if (builder.Configuration.GetValue<bool>("dataprotection:UseRedis"))
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(redisConfigurationOptions);
                builder.Services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UsePersist();

                if (builder.Configuration.GetValue<bool>("Fluxor:EnableReduxDevTools"))
                {
                    options.UseReduxDevTools();
                }
            });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<IStringStateStorage, SessionStateStorage>();
            builder.Services.AddScoped<IStoreHandler, JsonStoreHandler>();

            ISignalRServerBuilder signalRBuilder = builder.Services.AddSignalR();

            if (builder.Configuration.GetValue<bool>("signalr:UseRedisBackplane"))
            {
                signalRBuilder.AddStackExchangeRedis(options =>
                {
                    options.Configuration = redisConfigurationOptions;
                });
            }

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub(options =>
            {
                options.Transports = HttpTransportType.WebSockets;
            });

            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}