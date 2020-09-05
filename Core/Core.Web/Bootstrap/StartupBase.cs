// <copyright file="StartupBase.cs" company="NA">
//NA
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Core.Common.Web;
using Core.DTO;
using Core.Web.HealthCheckRegistry;
using Core.Web.ServiceRegistry;
using Core.Web.ServiceRegistry;
using Correlate.AspNetCore;
using Correlate.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.Management.CloudFoundry;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Hypermedia;
using ApplicationException = Core.DTO.ApplicationException;

namespace Core.Web
{
    /// <summary>
    /// Base startup class to be used for all the services.
    /// </summary>
    public abstract class StartupBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupBase"/> class.
        /// constructor.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public StartupBase(IConfiguration configuration)
        {
            this.Configuration = configuration;
            this.ServiceName = configuration.GetValue("ServiceName", string.Empty);
            StartTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the start time of  application.
        /// </summary>
        public static DateTime StartTime { get; private set; }

        /// <summary>
        /// gets the value of Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// gets the value of Service name.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Callled at runtime.
        /// </summary>
        /// <param name="services">collection service.</param>
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddCorrelate();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"{this.ServiceName} API", Version = "v1" });
                c.CustomSchemaIds(x => x.FullName);
            });
            services.AddCloudFoundryActuators(this.Configuration, MediaTypeVersion.V2, ActuatorContext.ActuatorAndCloudFoundry);
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var appException = new ApplicationException(context.ModelState.Values.SelectMany(v => v.Errors)
                       .Select(modelError => new ErrorDetail(StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture), modelError.ErrorMessage))
                       .ToList())
                    {
                        StatusCode = StatusCodes.Status400BadRequest.ToString(CultureInfo.InvariantCulture)
                    };
                    var response = new ObjectResult(new { appException.Id, appException.Errors, appException.StatusCode });
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    return response;
                };
            });

            var transientPolicySettings = new TransientPolicySettings(Configuration);
            services.AddSingleton(transientPolicySettings);
            services.RegisterInstace<IHttpRestClient, HttpRestClient>(transientPolicySettings);
            services.AddHealthChecks();
            services.AddCors();
            this.ConfigureComponentServices(services);
        }

        /// <summary>
        /// called at runtime.
        /// </summary>
        /// <param name="app">application builder.</param>
        /// <param name="env">host environment.</param>
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCorrelate();
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCorrelationHeader();
            app.UseCloudFoundryActuators(MediaTypeVersion.V2, ActuatorContext.ActuatorAndCloudFoundry);
            app.UseRequestResponseLogging();

            app.UseAppException();
            app.UseServiceStatus(env);
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}");
                endpoints.MapHealthChecks("/healthchecks", new HealthCheckOptions()
                {
                    ResultStatusCodes =
                    {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                    },
                    ResponseWriter = HealthChecksBuilder.WriteHealthCheckResponse
                });
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/Swagger.json", $"{this.ServiceName} API");
            });

            this.ConfigureComponent(app);
        }

        /// <summary>
        /// configure service specific dependencies.
        /// </summary>
        /// <param name="services">service collection.</param>
        public abstract void ConfigureComponentServices(IServiceCollection services);

        /// <summary>
        /// configure service specific pipeline/middleware.
        /// </summary>
        /// <param name="app">application builder.</param>
        public abstract void ConfigureComponent(IApplicationBuilder app);
    }
}
