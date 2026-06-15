using Newtonsoft.Json;

namespace Kifubara.Models;

/// <summary>Returned by DELETE /api/gomagic/games/{id}.</summary>
public class DeleteGameResponse
{
    [JsonProperty("deleted")]
    public bool Deleted { get; set; }

    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;
}