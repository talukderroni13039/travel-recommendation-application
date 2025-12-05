using System.Text.Json.Serialization;

namespace TravelRecommendation.Application.DTO.Responses
{
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
