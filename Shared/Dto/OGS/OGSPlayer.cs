using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Shared.Dto.OGS
{
    public class OGSPlayer
    {
        [JsonProperty("ratings/overall/rating")]
        public double Ranking { get; set; }

        [JsonProperty("ratings")]
        public JObject Ratings { get; set; }  // keep as JObject

        [JsonIgnore]
        public double Rating => Ratings?["overall"]?["rating"]?.Value<double>() ?? 0;
    }
}
