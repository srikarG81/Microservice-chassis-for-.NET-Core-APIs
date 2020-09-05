// <copyright file="Registry.cs" company="NA">
//NA
// </copyright>

using System.Net;
using System.Net.Http;
using Core.Web.ServiceRegistry;
using Correlate.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly.Wrap;

namespace Core.Web.ServiceRegistry
{
    /// <summary>
    /// It is used to register instances which will add polly transient policies.
    /// </summary>
    public static class Registry
    {
        /// <summary>
        /// Register instance in DI.
        /// </summary>
        /// <typeparam name="TClient">Interface implemented by a type.</typeparam>
        /// <typeparam name="TImplementation">A type implements TClient interface.</typeparam>
        /// <param name="services">instance of IServiceCollection.</param>
        /// <param name="transientPolicySettings">transient poicy settings.</param>
        public static void RegisterInstace<TClient, TImplementation>(this IServiceCollection services, TransientPolicySettings transientPolicySettings)
            where TClient : class
            where TImplementation : class, TClient
        {
            PolicyBuilder policyBuilder = new PolicyBuilder();
            var defaultPolicy = policyBuilder.GetDefaultPolicy(transientPolicySettings);
            services.AddHttpClient<TClient, TImplementation>()
               .AddPolicyHandler(t => { return defaultPolicy; })
               .CorrelateRequests("X-Correlation-ID")
               .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
               {
                   AllowAutoRedirect = false,
                   AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
               });
        }

        /// <summary>
        /// Register instance in DI.
        /// </summary>
        /// <typeparam name="TClient">Interface implemented by a type.</typeparam>
        /// <typeparam name="TImplementation">A type implements TClient interface.</typeparam>
        /// <param name="services">instance of IServiceCollection.</param>
        /// <param name="policy">circuit breaker policy.</param>
        public static void RegisterInstace<TClient, TImplementation>(this IServiceCollection services,  PolicyWrap<HttpResponseMessage> policy)
            where TClient : class
            where TImplementation : class, TClient
        {
            PolicyBuilder policyBuilder = new PolicyBuilder();         
            services.AddHttpClient<TClient, TImplementation>()
               .AddPolicyHandler(t => { return policy; })
               .CorrelateRequests("X-Correlation-ID")
               .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler()
               {
                   AllowAutoRedirect = false,
                   AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
               });
        }
    }
}
