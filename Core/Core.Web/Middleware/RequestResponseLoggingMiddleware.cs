// <copyright file="RequestResponseLoggingMiddleware.cs" company="NA">
//NA
// </copyright>

using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Core.Web
{
    /// <summary>
    /// Request and Response logging middleware.
    /// </summary>
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestResponseLoggingMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestResponseLoggingMiddleware"/> class.
        /// </summary>
        /// <param name="next"> request delegate.</param>
        /// <param name="logger">logger for logging.</param>
        public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// method called by middleware pipeline.
        /// </summary>
        /// <param name="context"> http context.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Request.EnableBuffering();

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[Convert.ToInt32(context.Request.ContentLength, CultureInfo.InvariantCulture)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);

            logger.LogInformation("Input ManageContact Request {@requestBody}", Encoding.UTF8.GetString(buffer));

            context.Request.Body.Seek(0, SeekOrigin.Begin);

            var originalResponseStream = context.Response.Body;
            using var newResponseStream = new MemoryStream();
            context.Response.Body = newResponseStream;

            await next(context).ConfigureAwait(false);

            newResponseStream.Seek(0, SeekOrigin.Begin);

            using var streamReader = new StreamReader(newResponseStream);

            logger.LogInformation("ManageContact Response {@response}", await streamReader.ReadToEndAsync());

            newResponseStream.Seek(0, SeekOrigin.Begin);

            await newResponseStream.CopyToAsync(originalResponseStream);
            context.Response.Body = originalResponseStream;
        }
    }
}
