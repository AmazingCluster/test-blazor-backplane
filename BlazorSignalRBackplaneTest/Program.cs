using BlazorSignalRBackplaneTest.Data;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

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

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}