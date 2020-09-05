// <copyright file="CorrelationHeaderMiddleware.cs" company="NA">
//NA
// </copyright>

namespace Core.Web
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Serilog.Context;

    /// <summary>
    /// Middleware to insert or retrieve Correlation Header.
    /// </summary>
    public class CorrelationHeaderMiddleware
    {
        private const string CORRELATION_ID_HEADER_NAME = "X-Correlation-ID";
        private readonly RequestDelegate next;
        private readonly ILogger<CorrelationHeaderMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CorrelationHeaderMiddleware"/> class.
        /// </summary>
        /// <param name="next"> request delegate.</param>
        /// <param name="logger">logger.</param>
        public CorrelationHeaderMiddleware(RequestDelegate next, ILogger<CorrelationHeaderMiddleware> logger)
        {
            this.next = next ?? throw new System.ArgumentNullException(nameof(next));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Middleware invoke method.
        /// </summary>
        /// <param name="context">http context.</param>
        /// <returns>task. <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new System.ArgumentNullException(nameof(context));
            }

            var correlationId = this.GetOrAddCorrelationHeader(context);

            try
            {
                // Add as many nested usings as needed, for adding more properties 
                using (LogContext.PushProperty(CORRELATION_ID_HEADER_NAME, correlationId, true))
                {
                    // LogContext.PushProperty("IPAddress", context.Connection.RemoteIpAddress, true);
                    await this.next.Invoke(context);
                }
            }

            // To make sure that we don't loose the scope in case of an unexpected error
            catch (Exception ex) when (this.LogOnUnexpectedError(ex))
            {
                return;
            }
        }

        private string GetOrAddCorrelationHeader(HttpContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Request.Headers[CORRELATION_ID_HEADER_NAME]))
            {
                context.Request.Headers[CORRELATION_ID_HEADER_NAME] = Guid.NewGuid().ToString();
            }

            return context.Request.Headers[CORRELATION_ID_HEADER_NAME];
        }

        private bool LogOnUnexpectedError(Exception ex)
        {
            this.logger.LogError(ex, "An unexpected exception occured!");
            return true;
        }
    }
}
