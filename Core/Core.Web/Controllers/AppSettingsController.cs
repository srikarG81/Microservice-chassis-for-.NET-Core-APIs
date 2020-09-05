// <copyright file="AppSettingsController.cs" company="NA">
//NA
// </copyright>

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Core.Web.Controllers
{
    /// <summary>
    /// controller for the appsettings endpoints.
    /// </summary>
    [Route("Configuration")]
    [ApiController]
    public class AppSettingsController : ControllerBase
    {
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppSettingsController"/> class.
        /// </summary>
        /// <param name="configuration">configuration.</param>
        public AppSettingsController(IConfiguration configuration)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            var val = configuration.GetSection("ESBUrls");
#pragma warning restore CA1062 // Validate arguments of public methods
            this.configuration = configuration;
        }

        /// <summary>
        /// Get configuration value.
        /// </summary>
        /// <param name="key">configuration key.</param>
        /// <returns>Configuration value.</returns>
        [HttpGet("Value/{key}")]
        public IActionResult GetConfigValues(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return BadRequest($"Config : {nameof(key)} can't be null or empty.");
            }

            return Ok(configuration.GetValue<string>(key, null) ?? $"{key} key not found in the configuration.");
        }

        /// <summary>
        /// reloads application configurations.
        /// </summary>
        /// <returns>returns successful response message.</returns>
        [HttpPost("Reload")]
        public ActionResult<string> ReloadAppSettings()
        {
            (configuration as IConfigurationRoot).Reload();

            return Ok("Successfully reloaded App settings.");
        }
    }
}