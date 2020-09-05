// <copyright file="MainProgram.cs" company="NA">
//NA
// </copyright>

using System;
using System.IO;
using System.Reflection;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Steeltoe.Common.Hosting;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.PlaceholderCore;
using Steeltoe.Extensions.Configuration.RandomValue;
using Steeltoe.Extensions.Logging;

namespace Core.Web
{
    /// <summary>
    /// Main method for starting service.
    /// </summary>
    public static class MainProgram
    {
        /// <summary>
        /// method for starting the webhost.
        /// </summary>
        /// <typeparam name="T"> class type.</typeparam>
        /// <param name="args">command line arguments.</param>
        public static void Run<T>(string[] args)
            where T : class
        {
            using var host = BuildWebHost<T>(args);

            host?.Run();
        }

        private static IWebHost BuildWebHost<T>(string[] args)
            where T : class
        {
            Assembly serviceAssembly = Assembly.GetEntryAssembly();
            var baseRoot = Path.GetDirectoryName(serviceAssembly.Location);
            Directory.SetCurrentDirectory(baseRoot);

            string appSettingFile = "appsettings.json";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(baseRoot)
                .AddJsonFile(appSettingFile, optional: false, reloadOnChange: true)           
                .Build();

            var env = configuration.GetValue("ASPNETCORE_ENVIRONMENT", "Development");

            var serviceName = configuration.GetValue<string>("ServiceName", serviceAssembly.FullName.Remove(serviceAssembly.FullName.IndexOf(',')));

            if (!string.IsNullOrEmpty(configuration.GetValue<string>("spring:application:name", null)))
            {
                SetCloudConfigFileName(env, appSettingFile, serviceName, baseRoot);
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Log.Logger.Information($"Application {serviceName} is starting..");
            try
            {
                var webHostBuilder = WebHost.CreateDefaultBuilder()
                    .UseDefaultServiceProvider(configure => configure.ValidateScopes = false)
                    .UseCloudFoundryHosting()
                    .AddConfigServer()
                    .ConfigureAppConfiguration((hostingContext, config) =>
                     {
                         var env = hostingContext.HostingEnvironment;
                         config.AddJsonFile($"appsettings.json");
                     })
                    .AddPlaceholderResolver()
                    .ConfigureAppConfiguration((b) => b.AddRandomValueSource())
                    .UseSerilog();

                webHostBuilder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog(Log.Logger);
                    logging.AddConsole();
                });

                webHostBuilder.ConfigureLogging((hostingContext, loggingBuilder) =>
                {
                    loggingBuilder.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    loggingBuilder.AddDynamicConsole();
                });

                return webHostBuilder.UseStartup<T>().Build();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application start-up failed");
                return null;
            }
        }

        private static void SetCloudConfigFileName(string env, string appSettingFile, string serviceName, string baseRoot)
        {
            var appSettingsJsonFilePath = System.IO.Path.Combine(baseRoot, appSettingFile);

            var json = System.IO.File.ReadAllText(appSettingsJsonFilePath);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);

            var filename = jsonObj["spring"]["application"]["name"].Value;
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }

            jsonObj["spring"]["application"]["name"] = filename.Replace("{ServiceName}", serviceName)
                .Replace("{Environment}", env);

            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText(appSettingsJsonFilePath, output);
        }
    }
}