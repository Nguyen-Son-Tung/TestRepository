using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Producers.ReponseContainers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Producers
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Producers", Version = "v1" });
            });


            // Start Config For Health Check
            services.AddHealthChecks()
                .AddCheck("OS Ready 2", () => HealthCheckResult.Healthy("Service Healthy"), tags: new[] { "ready" })
                .AddCheck("OS Ready", () => HealthCheckResult.Healthy("Service Healthy"), tags: new[] { "ready" })
                .AddRabbitMQ(rabbitConnectionString: "amqp://guest:guest@localhost:5672", tags: new[] { "ready" });


            // End Config For Health Check


            // Start Config For Masstransit
            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq();
            });

            services.AddMassTransitHostedService();

            // End Config Masstransit

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Producers v1"));
            }



            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // Config For Health Check
                endpoints.MapHealthChecks("/api/healthz",
                    new HealthCheckOptions()
                    {
                        ResultStatusCodes = new Dictionary<HealthStatus, int>()
                        {
                            { HealthStatus.Healthy, (int)HttpStatusCode.BadRequest },
                            { HealthStatus.Unhealthy, (int)HttpStatusCode.OK },
                            { HealthStatus.Degraded, (int)HttpStatusCode.NotFound }
                        },

                        Predicate = h => h.Tags.Contains("ready"),

                        ResponseWriter = ResponseWritterProvider.WriteResponse
                    });


                endpoints.MapHealthChecks("/api/health/live",
                    new HealthCheckOptions()
                    {
                        ResultStatusCodes = new Dictionary<HealthStatus, int>()
                        {
                            { HealthStatus.Healthy, (int)HttpStatusCode.BadRequest },
                            { HealthStatus.Unhealthy, (int)HttpStatusCode.OK },
                            { HealthStatus.Degraded, (int)HttpStatusCode.NotFound }
                        },

                        Predicate = _ => false,

                        ResponseWriter = ResponseWritterProvider.WriteResponse
                    });

                endpoints.MapControllers();
            });
        }
    }
}
