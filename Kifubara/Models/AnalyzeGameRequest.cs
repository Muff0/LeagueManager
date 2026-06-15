using Newtonsoft.Json;

namespace Kifubara.Models;

public class AnalyzeGameRequest
{
    /// <summary>The SGF text to analyze. Required.</summary>
    [JsonProperty("sgf")]
    public required string Sgf { get; set; }

    /// <summary>
    /// KataGo visits per move. Lower = faster, rougher analysis.
    /// Clamped to the account limit (2500). Defaults to the limit when null.
    /// </summary>
    [JsonProperty("visits")]
    public int? Visits { get; set; }

    /// <summary>Your own correlation ID for the match. Echoed back in responses.</summary>
    [JsonProperty("match_id")]
    public string? MatchId { get; set; }

    /// <summary>League name. Falls back to SGF's EV[] then GN[] when null.</summary>
    [JsonProperty("league")]
    public string? League { get; set; }

    /// <summary>Season label.</summary>
    [JsonProperty("season")]
    public string? Season { get; set; }

    /// <summary>Round. Falls back to SGF's RO[] when null.</summary>
    [JsonProperty("round")]
    public string? Round { get; set; }

    /// <summary>Where the game was played. Falls back to SGF's PC[] when null.</summary>
    [JsonProperty("external_url")]
    public string? ExternalUrl { get; set; }
}