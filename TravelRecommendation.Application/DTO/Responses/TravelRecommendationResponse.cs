using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelRecommendation.Application.DTO.Responses
{
    public class TravelRecommendationResponse
    {
        public string Recommendation { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public LocationComparison Comparison { get; set; } = new();
    }

    public class LocationComparison
    {
        public LocationWeatherInfo CurrentLocation { get; set; } = new();
        public LocationWeatherInfo Destination { get; set; } = new();
    }

    public class LocationWeatherInfo
    {
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Date { get; set; }
        public double TemperatureAt2PM { get; set; }
        public double Pm25At2PM { get; set; }
  
    }
}
