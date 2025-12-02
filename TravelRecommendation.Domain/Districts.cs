using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TravelRecommendation.Domain
{
        public class Districts
           {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("division_id")]
            public string DivisionId { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("bn_name")]
            public string BnName { get; set; }

            [JsonPropertyName("lat")]
        public double Latitude { get; set; }


        [JsonPropertyName("long")]
        public double Longitude { get; set; }
    }

}
