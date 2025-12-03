using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravelRecommendation.Application.DTO
{
    public class DistrictWeatherSummary
    {
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double AvgTemperature { get; set; }
        public double AvgPm25 { get; set; }
    }
}
