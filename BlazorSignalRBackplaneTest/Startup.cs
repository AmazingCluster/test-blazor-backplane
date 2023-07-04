using Blazored.LocalStorage;

using BlazorSignalRBackplaneTest.Data;
using BlazorSignalRBackplaneTest.State.Storage;

using Fluxor;
using Fluxor.Persist.Middleware;
using Fluxor.Persist.Storage;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Connections;

using StackExchange.Redis;

namespace BlazorSignalRBackplaneTest
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            services.AddHttpClient();
            services.AddLogging();
            
            if (Configuration.GetValue<bool>("Redis:Enabled"))
            {
                ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
                {
                    EndPoints = { { Configuration.GetValue<string>("redis:host"), Configuration.GetValue<int>("redis:port") } },
                    Password = Configuration.GetValue<string>("REDIS_PASSWORD"),
                    ServiceName = Configuration.GetValue<string>("redis:masterSet"),
                    AllowAdmin = Configuration.GetValue<bool>("redis:allowAdmin")
                });

                services
                    .AddDataProtection()
                    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");
            }

            services.AddFluxor(options =>
            {
                options.ScanAssemblies(typeof(Program).Assembly);
                options.UsePersist();

                if (Configuration.GetValue<bool>("Fluxor:EnableReduxDevTools"))
                {
                    options.UseReduxDevTools();
                }
            });

            services.AddBlazoredLocalStorage();
            services.AddScoped<IStringStateStorage, SessionStateStorage>();
            services.AddScoped<IStoreHandler, JsonStoreHandler>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (!environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapBlazorHub(options =>
                {
                    // Only websockets as we will be running in a RR loadbalanced environment and the fallback methods (Server Sent Events & Long polling) will not work 
                    options.Transports = HttpTransportType.WebSockets;
                });
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
