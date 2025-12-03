using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelRecommendation.Application.Services;

namespace TravelRecommendation.Application.DTO
{
    public class Top10Response
    {
        public DateTime GeneratedAt { get; set; }
        public List<DistrictRank> Districts { get; set; }
    }
    public class DistrictRank
    {
        public int Rank { get; set; }
        public string DistrictName { get; set; }
        public double AvgTemperature { get; set; }
        public double AvgPm25 { get; set; }
       // public string AirQualityStatus { get; set; }
    }
}
