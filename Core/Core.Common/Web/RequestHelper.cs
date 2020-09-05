// <copyright file="RequestHelper.cs" company="NA">
//NA
// </copyright>

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Core.Common.Web
{
    /// <summary>
    /// Contains utility method for Http request.
    /// </summary>
    public static class RequestHelper
    {
        /// <summary>
        /// Creates error response.
        /// </summary>
        /// <param name="httpStatusCode">http status code.</param>
        /// <param name="message">error message.</param>
        /// <returns>object result error response.</returns>
        public static ObjectResult CreateErroResponse(HttpStatusCode httpStatusCode, string message)
        {
            ApplicationException customError = new ApplicationException(
                new List<ErrorDetail> { new ErrorDetail(httpStatusCode.ToString(), message) });
            return new ObjectResult(customError) { StatusCode = (int)httpStatusCode };
        }

        /// <summary>
        /// Creates error response.
        /// </summary>
        /// <param name="httpStatusCode">http status code.</param>
        /// <param name="message">error message.</param>
        /// <param name="reasonPhrase">reason phrase.</param>
        /// <returns>Http response message error response.</returns>
        public static HttpResponseMessage CreateErroResponse(HttpStatusCode httpStatusCode, string message, string reasonPhrase = "")
        {
            ApplicationException customError = new ApplicationException(
                 new List<ErrorDetail> { new ErrorDetail(httpStatusCode.ToString(), message) });
            return new HttpResponseMessage(httpStatusCode) { Content = new StringContent(message), ReasonPhrase = reasonPhrase };
        }

        /// <summary>
        /// Creates name value collection from the list.
        /// </summary>
        /// <param name="key">value representing the key.</param>
        /// <param name="list">list of string.</param>
        /// <returns>name value collection.</returns>
        public static NameValueCollection CreateNameValueCollectionFromList(string key, List<string> list)
        {
            var nvCollection = new NameValueCollection();
            list?.ForEach(item => nvCollection.Add(key, item));
            return nvCollection;
        }
    }
}
