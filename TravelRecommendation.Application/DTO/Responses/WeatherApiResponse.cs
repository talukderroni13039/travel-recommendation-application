using System.Text.Json.Serialization;

namespace TravelRecommendation.Application.DTO.Responses
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
}
