
using Newtonsoft.Json;
namespace TravelRecommendation.Domain
{

    public class DistrictsRoot
    {
        [JsonProperty("districts")]
        public List<Districts> Districts { get; set; } = new List<Districts>();
    }


public class Districts
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("division_id")]
        public string DivisionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bn_name")]
        public string BnName { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("long")]
        public double Longitude { get; set; }
    }

}
