using Newtonsoft.Json;

namespace Kifubara.Models;

/// <summary>
/// Returned by POST /api/gomagic/analyze.
/// HTTP 201 on first submission, 200 when <see cref="Deduped"/> is true (same SGF already submitted).
/// </summary>
public class AnalyzeGameResponse
{
    /// <summary>GoMagic's ID for the game.</summary>
    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;

    /// <summary>Public analysis page URL.</summary>
    [JsonProperty("share_url")]
    public string ShareUrl { get; set; } = string.Empty;

    /// <summary>URL to poll for analysis progress.</summary>
    [JsonProperty("state_url")]
    public string StateUrl { get; set; } = string.Empty;

    /// <summary>Echoed back from the request's <see cref="AnalyzeGameRequest.MatchId"/>.</summary>
    [JsonProperty("match_id")]
    public string? MatchId { get; set; }

    /// <summary>True if this SGF was already submitted; no re-analysis was triggered.</summary>
    [JsonProperty("deduped")]
    public bool Deduped { get; set; }
}