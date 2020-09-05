// <copyright file="PolicyBuilder.cs" company="NA">
//NA
// </copyright>
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoAPI
{
    public class WeatherForecast
    {
        [Required(ErrorMessage = "Required Name filed")]
        String? Name { get; set; }
        [Required( ErrorMessage = "Required Date filed")]
        [Range(typeof(DateTime), "1/2/2004", "3/4/2025",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
