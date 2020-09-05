// <copyright file="PolicyBuilder.cs" company="NA">
//NA
// </copyright>

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Web.ServiceRegistry;
using Polly;
using Polly.Extensions.Http;
using Polly.Timeout;
using Polly.Wrap;
using Serilog;

namespace Core.Web.ServiceRegistry
{
    /// <summary>
    /// Policy builder for circuit breaker.
    /// </summary>
    public class PolicyBuilder
    {
        /// <summary>
        /// Build default circuit breaker, wait and retry and Timeout policy. 
        /// </summary>
        /// <param name="transientPolicySettings">retry,timeout and circuit breaker settings.</param>
        /// <returns>policy wrapper.</returns>
        public virtual PolicyWrap<HttpResponseMessage> GetDefaultPolicy(TransientPolicySettings transientPolicySettings)
        {
            //Time out policy to set Http Client request timeout.
            TimeoutPolicy timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(transientPolicySettings.HttpTimeOut), timeoutStrategy: TimeoutStrategy.Pessimistic, TimeOutHandler);
            //If http client requiest fails, it will be retried.
            IAsyncPolicy<HttpResponseMessage> retryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                                                           .Or<HttpRequestException>()
                                                           .WaitAndRetryAsync(transientPolicySettings.RetryAttempts,retryafter => TimeSpan.FromSeconds(Math.Pow(2, retryafter) / 2), OnRetry);
            //Circuit breaker to fail fast. 
            IAsyncPolicy<HttpResponseMessage> cb = HttpPolicyExtensions
                                                   .HandleTransientHttpError()
                                                   .AdvancedCircuitBreakerAsync(
                                                       transientPolicySettings.CircuitBreakerSettings.FailureThreshold,
                                                       TimeSpan.FromSeconds(transientPolicySettings.CircuitBreakerSettings.SamplingDuration),
                                                       transientPolicySettings.CircuitBreakerSettings.MinimumThroughput,
                                                       TimeSpan.FromSeconds(transientPolicySettings.CircuitBreakerSettings.DurationOfBreak),
                                                       OnBreak,
                                                       OnReset,
                                                       OnHalfOpen);
            //above policies are wrapped.
            return timeoutPolicy.WrapAsync(retryPolicy.WrapAsync(cb));
        }

        /// <summary>
        /// Triggers when circuit is half open.
        /// </summary>
        public virtual void OnHalfOpen()
        {
            Log.Warning("Circuit breaker state is changed to OnHalfOpen");
        }

        /// <summary>
        /// Triggers when circuit is closed.
        /// </summary>
        public virtual void OnReset()
        {
            Log.Warning("Circuit breaker state is changed to close");
        }

        /// <summary>
        /// Triggers when circuit is open.
        /// </summary>
        /// <param name="delegateResult">contains exception details.</param>
        /// <param name="timeSpan">time span.</param>
        public virtual void OnBreak(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan)
        {
            Log.Warning("Circuit breaker state is changed to close");
            Log.Warning("Circuit breaker state is changed to close {@delegateResult.Exception}", delegateResult?.Exception);
        }

        /// <summary>
        /// Triggers before retrying the task.
        /// </summary>
        /// <param name="delegateResult">contains exception details.</param>
        /// <param name="timeSpan">time span.</param>
        public virtual void OnRetry(DelegateResult<HttpResponseMessage> delegateResult, TimeSpan timeSpan)
        {
            Log.Warning("OnRetry {@delegateResult.Exception}", delegateResult?.Exception);
        }

        /// <summary>
        /// Triggered when timeout occurs.
        /// </summary>
        /// <param name="context">Http context.</param>
        /// <param name="time">time span.</param>
        /// <param name="task">task to be executed.</param>
        /// <returns>task representing the result.</returns>
        public virtual Task TimeOutHandler(Context context, TimeSpan time, Task task)
        {
            return Task.Run(() =>
            {
                Log.Warning("timeOutHandler {time}", time);
            });
        }
    }
}
