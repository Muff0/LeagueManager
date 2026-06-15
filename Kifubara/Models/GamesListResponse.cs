using Newtonsoft.Json;

namespace Kifubara.Models;

/// <summary>Returned by GET /api/gomagic/games.</summary>
public class GamesListResponse
{
    [JsonProperty("items")]
    public List<GameStateResponse> Items { get; set; } = [];

    /// <summary>Number of items on this page.</summary>
    [JsonProperty("count")]
    public int Count { get; set; }

    /// <summary>Total games matching the applied filter.</summary>
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonProperty("limit")]
    public int Limit { get; set; }

    [JsonProperty("offset")]
    public int Offset { get; set; }

    public bool HasMore => Offset + Count < Total;
}