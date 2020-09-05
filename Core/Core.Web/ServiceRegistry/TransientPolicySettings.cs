// <copyright file="TransientPolicySettings.cs" company="NA">
//NA
// </copyright>
using Microsoft.Extensions.Configuration;

namespace Core.Web.ServiceRegistry
{
    /// <summary>
    /// TransientPolicySettings like Retry, Circuit breaker and Http Client TimeOut.
    /// </summary>
    public class TransientPolicySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientPolicySettings"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public TransientPolicySettings(IConfiguration configuration)
        {
            this.HttpTimeOut = configuration?.GetValue<int>("HttpTimeOut", 15) ?? 15;
            this.RetryAttempts = configuration?.GetValue<int>("RetryAttempts", 3) ?? 3;
            CircuitBreakerSettings = new CircuitBreakerSettings(configuration);
        }

        /// <summary>
        /// Http client timeout.
        /// </summary>
        public int HttpTimeOut { get; set; }

        /// <summary>
        /// No.of retry attempts.
        /// </summary>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// Cb settings.
        /// </summary>
        public CircuitBreakerSettings CircuitBreakerSettings { get; set; }
    }

    /// <summary>
    /// Circuit breaker settings.
    /// </summary>
    public class CircuitBreakerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CircuitBreakerSettings"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public CircuitBreakerSettings(IConfiguration configuration)
        {
            this.FailureThreshold = configuration?.GetValue<double>("CircuitBreaker:FailureThreshold", 0.5) ?? 0.5;
            this.SamplingDuration = configuration?.GetValue<int>("CircuitBreaker:SamplingDuration", 60) ?? 60;
            this.MinimumThroughput = configuration?.GetValue<int>("CircuitBreaker:MinimumThroughput", 7) ?? 7;
            this.DurationOfBreak = configuration?.GetValue<int>("CircuitBreaker:DurationOfBreak", 60) ?? 60;
        }

        /// <summary>
        /// he failure threshold at which the circuit will break (a number between 0 and
        /// 1; eg 0.5 represents breaking if 50% or more of actions result in a handled failure.
        /// </summary>
        public double FailureThreshold { get; set; }

        /// <summary>
        /// The duration of the timeslice over which failure ratios are assessed.
        // </summary>
        public int SamplingDuration { get; set; }

        /// <summary>
        /// minimumThroughput:
        /// The minimum throughput: this many actions or more must pass through the circuit
        /// in the timeslice, for statistics to be considered significant and the circuit-breaker
        /// to come into action.
        /// </summary>
        public int MinimumThroughput { get; set; }

        /// <summary>
        /// The duration the circuit will stay open before resetting.
        /// </summary>
        public int DurationOfBreak { get; set; }
    }
}
