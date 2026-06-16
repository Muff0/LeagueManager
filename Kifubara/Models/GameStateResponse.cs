using Newtonsoft.Json;

namespace Kifubara.Models;

/// <summary>
/// Analysis state for a single game. Returned by GET /api/gomagic/games/{id}/state
/// and as items in GET /api/gomagic/games.
/// </summary>
public class GameStateResponse
{
    [JsonProperty("game_id")]
    public string GameId { get; set; } = string.Empty;

    /// <summary>
    /// One of: <see cref="KifubaraGameAnalysisState.Queued"/>, <see cref="KifubaraGameAnalysisState.Analyzing"/>,
    /// <see cref="KifubaraGameAnalysisState.Done"/>, <see cref="KifubaraGameAnalysisState.Error"/>.
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    /// <summary>Progress 0–100 while analyzing; null otherwise.</summary>
    [JsonProperty("pct")]
    public int? Pct { get; set; }

    [JsonProperty("share_url")]
    public string? ShareUrl { get; set; }

    [JsonProperty("match_id")]
    public string? MatchId { get; set; }

    /// <summary>Error description when <see cref="State"/> is <see cref="KifubaraGameAnalysisState.Error"/>.</summary>
    [JsonProperty("failure_message")]
    public string? FailureMessage { get; set; }

    /// <summary>
    /// When state is error: false = bad SGF (permanent), true = transient (will retry).
    /// Null otherwise.
    /// </summary>
    [JsonProperty("retryable")]
    public bool? Retryable { get; set; }

    public bool IsDone    => State == KifubaraGameAnalysisState.Done;
    public bool IsError   => State == KifubaraGameAnalysisState.Error;
    public bool IsComplete => IsDone || IsError;
}