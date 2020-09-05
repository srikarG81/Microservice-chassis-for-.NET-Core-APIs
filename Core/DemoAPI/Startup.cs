// <copyright file="PolicyBuilder.cs" company="NA">
//NA
// </copyright>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Web.HealthCheckRegistry;
using Core.Web.ServiceRegistry;
using DemoAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DemoAPI
{
    public class Startup : Core.Web.StartupBase
    {
        public Startup(IConfiguration configuration)
           : base(configuration)
        {
        }

        public override void ConfigureComponentServices(IServiceCollection services)
        {
            var transientPolicySettings = new TransientPolicySettings(base.Configuration);
            services.RegisterInstace<IProductService, ProductService>(transientPolicySettings);

            // Add health checks for external services.
            Dictionary<string, string> externalServices = new Dictionary<string, string>();
            //Add external service name and URL
            externalServices.Add("ProductService", "http://localhost:59520");
            services.RegisterHealthChecksForExternalUris(externalServices);
        }

        public override void ConfigureComponent(IApplicationBuilder app)
        {

        }
    }
}
