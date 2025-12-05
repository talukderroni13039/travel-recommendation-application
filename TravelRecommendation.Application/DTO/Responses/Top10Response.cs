
namespace TravelRecommendation.Application.DTO.Responses
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
    }
}
