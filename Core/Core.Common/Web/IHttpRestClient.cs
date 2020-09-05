// <copyright file="IHttpRestClient.cs" company="NA">
//NA
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Common.Web
{
    /// <summary>
    /// interface for rest http calls.
    /// </summary>
    public interface IHttpRestClient
    {
        /// <summary>
        /// makes an http delete call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        Task<T> CallDeleteApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null);

        /// <summary>
        /// makes an http get call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param> 
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        Task<T> CallGetApi<T>(Uri url, NameValueCollection queryParams = null, Dictionary<string, string> headers = null);

        /// <summary>
        /// makes an http post call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        Task<T> CallPostApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null);

        /// <summary>
        /// makes an http put call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        Task<T> CallPutApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null);

        /// <summary>
        /// makes an http call.
        /// </summary>
        /// <param name="url">url of the service.</param>
        /// <param name="httpMethod">Http method.</param>
        /// <param name="payload">body payload for put and post.</param>
        /// <param name="queryCollection">query parameters.</param>
        /// <param name="headerDict">header values.</param>
        /// <param name="credentials">credentials for authentication.</param>
        /// <returns> Response message, A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<HttpResponseMessage> SendAsync(Uri url, HttpMethod httpMethod, string payload = null, NameValueCollection queryCollection = null, Dictionary<string, string> headerDict = null, ICredentials credentials = null);

        /// <summary>
        /// sete the json Serialization settins for the payload content serialization.
        /// </summary>
        /// <param name="serializerSettings">JsonSerializerSettings.</param>
        void SetJsonSerializerSettings(JsonSerializerSettings serializerSettings);
    }
}