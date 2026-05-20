using System.Text.Json.Serialization;

namespace Shared.Dto.OGS
{
    public class GetPlayersResult
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
        [JsonPropertyName("results")]
        public OGSPlayer[] Results{ get; set; }
    }
}
