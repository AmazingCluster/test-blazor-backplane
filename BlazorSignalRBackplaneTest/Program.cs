using Blazored.LocalStorage;

using BlazorSignalRBackplaneTest.Data;
using BlazorSignalRBackplaneTest.State.Storage;

using Fluxor;
using Fluxor.Persist.Middleware;
using Fluxor.Persist.Storage;

using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;

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

            builder.Services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UsePersist();
#if DEBUG
                options.UseReduxDevTools();
#endif
            });

            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddScoped<IStringStateStorage, SessionStateStorage>();
            builder.Services.AddScoped<IStoreHandler, JsonStoreHandler>();

            ISignalRServerBuilder signalRBuilder = builder.Services.AddSignalR();

            if (builder.Configuration.GetValue<bool>("signalr:UseRedisBackplane"))
            {
                signalRBuilder.AddStackExchangeRedis(options =>
                {
                    options.Configuration = new StackExchange.Redis.ConfigurationOptions
                    {
                        EndPoints = { { builder.Configuration.GetValue<string>("signalr:redis:host"), builder.Configuration.GetValue<int>("signalr:redis:port") } },
                        Password = builder.Configuration.GetValue<string>("REDIS_PASSWORD"),
                        ServiceName = builder.Configuration.GetValue<string>("signalr:redis:masterSet"),
                        AllowAdmin = builder.Configuration.GetValue<bool>("signalr:redis:allowAdmin")
                    };
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