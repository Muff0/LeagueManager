
using Newtonsoft.Json;
namespace OGS.Model;


public class GameDetail
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("all_players")]
    public List<int> AllPlayers { get; set; } = [];

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("players")]
    public string Players { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("related")]
    public string Related { get; set; } = null!;

    [JsonProperty("creator")]
    public int Creator { get; set; }

    [JsonProperty("mode")]
    public string Mode { get; set; } = null!;

    [JsonProperty("source")]
    public string Source { get; set; } = null!;

    [JsonProperty("black")]
    public int? Black { get; set; }

    [JsonProperty("white")]
    public int? White { get; set; }

    [JsonProperty("width")]
    public int Width { get; set; }

    [JsonProperty("height")]
    public int Height { get; set; }

    [JsonProperty("rules")]
    public string Rules { get; set; } = null!;

    [JsonProperty("ranked")]
    public bool Ranked { get; set; }

    [JsonProperty("handicap")]
    public int Handicap { get; set; }

    [JsonProperty("handicap_rank_difference")]
    public string? HandicapRankDifference { get; set; }

    [JsonProperty("komi")]
    public string? Komi { get; set; }

    [JsonProperty("time_control")]
    public string TimeControl { get; set; } = null!;

    [JsonProperty("black_player_rank")]
    public int BlackPlayerRank { get; set; }

    [JsonProperty("black_player_rating")]
    public string BlackPlayerRating { get; set; } = null!;

    [JsonProperty("white_player_rank")]
    public int WhitePlayerRank { get; set; }

    [JsonProperty("white_player_rating")]
    public string WhitePlayerRating { get; set; } = null!;

    [JsonProperty("time_per_move")]
    public int TimePerMove { get; set; }

    [JsonProperty("time_control_parameters")]
    public string? TimeControlParameters { get; set; }

    [JsonProperty("disable_analysis")]
    public bool DisableAnalysis { get; set; }

    [JsonProperty("tournament")]
    public int? Tournament { get; set; }

    [JsonProperty("tournament_round")]
    public int TournamentRound { get; set; }

    [JsonProperty("ladder")]
    public int? Ladder { get; set; }

    [JsonProperty("pause_on_weekends")]
    public bool PauseOnWeekends { get; set; }

    [JsonProperty("outcome")]
    public string Outcome { get; set; } = null!;

    [JsonProperty("black_lost")]
    public bool BlackLost { get; set; }

    [JsonProperty("white_lost")]
    public bool WhiteLost { get; set; }

    [JsonProperty("annulled")]
    public bool Annulled { get; set; }

    /// <summary>No schema defined by OGS — may be a string or object.</summary>
    [JsonProperty("annulment_reason")]
    public object? AnnulmentReason { get; set; }

    [JsonProperty("started")]
    public DateTimeOffset? Started { get; set; }

    [JsonProperty("ended")]
    public DateTimeOffset? Ended { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("historical_ratings")]
    public string HistoricalRatings { get; set; } = null!;

    /// <summary>Read-only. Full game state as a JSON string.</summary>
    [JsonProperty("gamedata")]
    public string Gamedata { get; set; } = null!;

    /// <summary>
    /// Read-only. Auth token for this game.
    /// Required when connecting to the OGS realtime Socket.IO API:
    /// emit game/connect with { game_id, player_id, auth }.
    /// </summary>
    [JsonProperty("auth")]
    public string Auth { get; set; } = null!;

    [JsonProperty("rengo")]
    public bool Rengo { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("flags")]
    public string Flags { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("bot_detection_results")]
    public string BotDetectionResults { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("simul_black")]
    public string SimulBlack { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("simul_white")]
    public string SimulWhite { get; set; } = null!;
}