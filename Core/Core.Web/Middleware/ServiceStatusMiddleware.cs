// <copyright file="ServiceStatusMiddleware.cs" company="NA">
//NA
// </copyright>

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Core.Web
{
    /// <summary>
    /// middleware for the service status api.
    /// </summary>
    public class ServiceStatusMiddleware
    {
        private readonly IWebHostEnvironment hostEnvironment;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStatusMiddleware"/> class.
        /// </summary>
        /// <param name="hostEnvironment">IhostEnvironment.</param>
        public ServiceStatusMiddleware(IWebHostEnvironment hostEnvironment)
        {
            this.hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// method called by middleware pipeline.
        /// </summary>
        /// <param name="httpContext">http context object.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            TimeSpan span = DateTime.UtcNow - StartupBase.StartTime;
            await httpContext.Response.WriteAsync($" {hostEnvironment.ApplicationName} Service is up and running since {span.Days}D_{span.Hours}H_{span.Minutes}M_{span.Seconds}S in {hostEnvironment.EnvironmentName} environment");
        }
    }
}
