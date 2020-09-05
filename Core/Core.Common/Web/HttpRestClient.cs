// <copyright file="HttpRestClient.cs" company="NA">
//NA
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Core.Common.Web
{
    /// <summary>
    /// class for rest http calls.
    /// </summary>
    public class HttpRestClient : IHttpRestClient
    {
        private readonly HttpClient httpClient;

        private JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRestClient"/> class.
        /// </summary>
        /// <param name="httpClient">http Client.</param>
        public HttpRestClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// makes an http get call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param> 
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        public async Task<T> CallGetApi<T>(Uri url, NameValueCollection queryParams = null, Dictionary<string, string> headers = null)
        {
            return await CallHttpService<T>(url, HttpMethod.Get, queryParams: queryParams, headers: headers);
        }

        /// <summary>
        /// makes an http post call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        public async Task<T> CallPostApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null)
        {
            return await CallHttpService<T>(url, HttpMethod.Post, payload: payload, queryParams: queryParams, headers: headers);
        }

        /// <summary>
        /// makes an http put call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        public async Task<T> CallPutApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null)
        {
            return await CallHttpService<T>(url, HttpMethod.Put, payload: payload, queryParams: queryParams, headers: headers);
        }

        /// <summary>
        /// makes an http delete call to the service.
        /// </summary>
        /// <typeparam name="T">genric type representing the response.</typeparam>
        /// <param name="url">url of the endpoint.</param>
        /// <param name="payload">body payload.</param>
        /// <param name="queryParams">query parameters.</param>
        /// <param name="headers">headers.</param>
        /// <returns>Async result of generic type T.</returns>
        public async Task<T> CallDeleteApi<T>(Uri url, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null)
        {
            return await CallHttpService<T>(url, HttpMethod.Delete, payload: payload, queryParams: queryParams, headers: headers);
        }

        private async Task<T> CallHttpService<T>(Uri url, HttpMethod httpMethod, object payload = null, NameValueCollection queryParams = null, Dictionary<string, string> headers = null)
        {
            var response = await SendAsync(url, httpMethod, payload == null ? null : Serialize(payload), queryParams, headers);

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
            }

            throw new Exception($"Call failed with status code {response.StatusCode}, reason phrase {response.ReasonPhrase}, errormessage {await response.Content.ReadAsStringAsync()}");
        }

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
        public async Task<HttpResponseMessage> SendAsync(Uri url, HttpMethod httpMethod, string payload = null, NameValueCollection queryCollection = null, Dictionary<string, string> headerDict = null, ICredentials credentials = null)
        {
            if (url == null)
            {
                throw new ArgumentException($"{nameof(url)} cannot be null or empty");
            }

            if (queryCollection != null)
            {
                url = new UriBuilder(url)
                {
                    Query = string.Join("&", queryCollection
                                                     .Cast<string>()
                                                     .Select(name => GetCollectionValues(queryCollection[name], name)).ToArray())
                }.Uri;
            }

            using var httpRequestMessage = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = url
            };

            if (payload != null)
            {
                httpRequestMessage.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            }

            httpRequestMessage.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            headerDict?.Keys.ToList().ForEach(key => httpRequestMessage.Headers.Add(key, headerDict[key]));            

            try
            {
                var data = await httpClient.SendAsync(httpRequestMessage);
                return data;
            }
            catch (TaskCanceledException ex)
            {
                throw new Exception("Request timeout" + ex.ToString());
            }
            catch (Exception)
            {
                throw;
            }
        }        

        /// <summary>
        /// sete the json Serialization settins for the payload content serialization.
        /// </summary>
        /// <param name="serializerSettings">JsonSerializerSettings.</param>
        public void SetJsonSerializerSettings(JsonSerializerSettings serializerSettings)
        {
            this.jsonSerializerSettings = serializerSettings;
        }

        private string Serialize(dynamic requestMessage)
        {
            return JsonConvert.SerializeObject(
                requestMessage,
                this.jsonSerializerSettings);
        }

        private string GetCollectionValues(string values, string name)
        {
            var querystring = string.Empty;
            values.Split(',').ToList().ForEach(value =>
            {
                querystring += $"{name}={value}&";
            });
            return querystring.Remove(querystring.Length - 1, 1);
        }
    }
}
