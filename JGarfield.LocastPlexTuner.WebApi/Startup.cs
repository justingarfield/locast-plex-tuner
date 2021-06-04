using JGarfield.LocastPlexTuner.Library;
using JGarfield.LocastPlexTuner.Library.Clients;
using JGarfield.LocastPlexTuner.Library.Clients.Contracts;
using JGarfield.LocastPlexTuner.Library.Services;
using JGarfield.LocastPlexTuner.Library.Services.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;

namespace JGarfield.LocastPlexTuner.WebApi
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
                    .AddXmlSerializerFormatters();

            // TODO: Figure out how to properly hook-in Prometheus IHttpClientFactory metrics here
            services.AddHttpClient()
                    .AddHttpClientLogging(_configuration)
                    .AddSingleton<IIpInfoClient, IpInfoClient>()
                    .AddSingleton<ILocastClient, LocastClient>()
                    .AddSingleton<IM3UService, M3UService>()
                    .AddSingleton<IEpg2XmlService, Epg2XmlService>()
                    .AddSingleton<IInitializationService, InitializationService>()
                    .AddSingleton<IIpInfoService, IpInfoService>()
                    .AddSingleton<IHttpFfmpegService, HttpFfmpegService>()
                    .AddSingleton<ILocastService, LocastService>()
                    .AddSingleton<IStationsService, StationsService>()
                    .AddSingleton<ITunerService, TunerService>()
                    .AddSingleton<ApplicationContext>()
                    .AddHostedService<LocastHostedService>()
                    .AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<RequestLoggingMiddleware>();
            }

            app.UseRouting();

            ConfigurePrometheus(app);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        /// Configures Metrics and Endpoint(s) for Prometheus.
        /// </summary>
        /// <param name="app"></param>
        private static void ConfigurePrometheus(IApplicationBuilder app)
        {
            // Prometheus ASP.NET Core HTTP request metrics (https://github.com/prometheus-net/prometheus-net#aspnet-core-http-request-metrics)
            app.UseHttpMetrics();

            app.UseEndpoints(endpoints =>
            {
                // Prometheus ASP.NET Core exporter middleware (https://github.com/prometheus-net/prometheus-net#aspnet-core-exporter-middleware)
                endpoints.MapMetrics();
            });
        }
    }
}
