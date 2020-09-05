// <copyright file="MiddlewareExtension.cs" company="NA">
//NA
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace Core.Web
{
    /// <summary>
    /// Extension class for the middleware.
    /// </summary>
    public static class MiddlewareExtension
    {
        /// <summary>
        /// Extension method for the exception middleware.
        /// </summary>
        /// <param name="builder">application builder.</param>
        /// <returns>application builder.</returns>
        public static IApplicationBuilder UseAppException(this IApplicationBuilder builder)
            => builder.UseMiddleware<GlobalExceptionMiddleware>();

        /// <summary>
        /// Extension method for service status middleware.
        /// </summary>
        /// <param name="builder">application builder.</param>
        /// <param name="env">Web Host Environemt.</param>
        /// <returns>application builder.</returns>
        public static IApplicationBuilder UseServiceStatus(this IApplicationBuilder builder, IWebHostEnvironment env)

            => builder.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.Equals("/service-status"), appBuilder =>
            {
                appBuilder.Run(async context => { await new ServiceStatusMiddleware(env).Invoke(context); });
            });

        /// <summary>
        /// Extension method for service correlation header middleware.
        /// </summary>
        /// <param name="builder">application builder.</param>
        /// <returns>application builder.</returns>
        public static IApplicationBuilder UseCorrelationHeader(this IApplicationBuilder builder)
            => builder.UseMiddleware<CorrelationHeaderMiddleware>();

        /// <summary>
        /// Extension method for service request response logging middleware.
        /// </summary>
        /// <param name="builder">application builder.</param>
        /// <returns>application builder.</returns>
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}