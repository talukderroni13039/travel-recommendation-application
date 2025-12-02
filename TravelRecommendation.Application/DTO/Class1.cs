using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TravelRecommendation.Application.DTO
{
    public class WeatherApiResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("hourly")]
        public WeatherHourlyData Hourly { get; set; }
    }

    public class WeatherHourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }

        [JsonPropertyName("temperature_2m")]
        public List<double?> Temperature2m { get; set; }
    }
    public class AirQualityApiResponse
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("hourly")]
        public AirQualityHourlyData Hourly { get; set; }
    }

    public class AirQualityHourlyData
    {
        [JsonPropertyName("time")]
        public List<string> Time { get; set; }

        [JsonPropertyName("pm2_5")]
        public List<double?> Pm25 { get; set; }
    }
}
