// <copyright file="HealthChecksBuilder.cs" company="NA">
//NA
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Web.HealthCheckRegistry
{
    /// <summary>
    /// Health Check builder.
    /// </summary>
    public static class HealthChecksBuilder
    {
        /// <summary>
        /// Registers the health check api.
        /// </summary>
        /// <typeparam name="T">a genertic representing the health check sevice type.</typeparam>
        /// <param name="services">service collection.</param>
        /// <param name="name">name of the service.</param>
        public static void RegisterHealthChecks<T>(this IServiceCollection services, string name)
            where T : class, IHealthCheck
        {
            services.AddHealthChecks().AddCheck<T>(name);
        }

        /// <summary>
        /// Registers the health check for external urls.
        /// </summary>
        /// <param name="services">service collection.</param>
        /// <param name="uris">external urls.</param>
        public static void RegisterHealthChecksForExternalUris(this IServiceCollection services, Dictionary<string, string> uris)
        {
            foreach (var item in uris)
            {
                services.AddHealthChecks().AddUrlGroup(new Uri(item.Value), item.Key, HealthStatus.Degraded, timeout: new TimeSpan(0, 0, 5));
            }
        }

        /// <summary>
        /// Builds health check response.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <param name="report">Health check response report.</param>
        /// <returns>Http response for health check endpoint.</returns>
        public static Task WriteHealthCheckResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";
            var data = new
            {
                OverallStatus = report.Status.ToString(),
                DependencyStatus = report.Entries.Select(t => new { Name = t.Key, Status = t.Value.Status.ToString() })
            };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(data);
            return context.Response.WriteAsync(jsonString.ToString());
        }
    }
}
