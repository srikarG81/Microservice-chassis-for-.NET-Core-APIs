// <copyright file="GlobalExceptionMiddleware.cs" company="NA">
//NA
// </copyright>

using System;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

using Core.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ApplicationException = Core.DTO.ApplicationException;

namespace Core.Web
{
    /// <summary>
    /// Exception middleware to catch all uncaught errors at the service level.
    /// </summary>
    public class GlobalExceptionMiddleware
    {
        private readonly ILogger<GlobalExceptionMiddleware> logger;
        private readonly RequestDelegate next;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.
        /// </summary>
        /// <param name="next">request delegate.</param>
        /// <param name="logger">logger.</param>
        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            this.logger = logger;
            this.next = next;
        }

        /// <summary>
        /// get called by the middleware pipeline.
        /// </summary>
        /// <param name="httpContext">http context object.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await next(httpContext); // calling next middleware
            }
            catch (ApplicationException ex)
            {
                if (ex is ValidationException)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
                else
                {
                    if (Enum.TryParse<HttpStatusCode>(ex.StatusCode, true, out HttpStatusCode httpStatusCode))
                    {
                        httpContext.Response.StatusCode = (int)httpStatusCode;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    }
                }

                ex.StatusCode = httpContext.Response.StatusCode.ToString(CultureInfo.InvariantCulture);
                await httpContext.Response.WriteAsync(CreateErrorResponse(ex, httpContext));
            }
            catch (Exception ex)
            {
                var appException = new ApplicationException()
                    .AddExceptionDetail(new ErrorDetail(StatusCodes.Status500InternalServerError.ToString(CultureInfo.InvariantCulture), "Something is broken."));

                this.logger.LogError("Error::{@Id}::{@ex}", appException.Id, ex);

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await httpContext.Response.WriteAsync(CreateErrorResponse(appException, httpContext));
            }
        }

        /// <summary>
        /// Create Json error response.
        /// </summary>
        /// <param name="appException">Application exception.</param>
        /// <returns>json string.</returns>
        private string CreateErrorResponse(ApplicationException appException, HttpContext httpContext)
        {
            appException.StatusCode = httpContext.Response.StatusCode.ToString(CultureInfo.InvariantCulture);
            return JsonConvert.SerializeObject(new { appException.Id, appException.Errors, appException.StatusCode });
        }
    }
}
