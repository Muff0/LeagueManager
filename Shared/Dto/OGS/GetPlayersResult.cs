using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
