using BlazorSignalRBackplaneTest.Data;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

            ISignalRServerBuilder signalRBuilder = builder.Services.AddSignalR();

            if(builder.Configuration.GetValue<bool>("signalr:UseRedisBackplane"))
            {
                signalRBuilder.AddStackExchangeRedis(options =>
                {
                    options.Configuration.Password = builder.Configuration.GetConnectionString("signalr:redisconnectionstring");
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

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}