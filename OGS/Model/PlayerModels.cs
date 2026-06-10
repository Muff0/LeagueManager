using Newtonsoft.Json;

namespace OGS.Model;

/// <summary>
/// Condensed player record returned by the players list/search endpoint.
/// Several read-only fields (ratings, ranking, icon, ui_class) are serialized as
/// JSON-encoded strings by OGS rather than structured objects.
/// </summary>
public class MinimalPlayer
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("country")]
    public string? Country { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("icon")]
    public string Icon { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("ratings")]
    public string Ratings { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("ranking")]
    public string Ranking { get; set; } = null!;

    [JsonProperty("professional")]
    public bool Professional { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("ui_class")]
    public string UiClass { get; set; } = null!;
}

/// <summary>
/// Full player profile.
/// </summary>
public class Player
{
    /// <summary>Read-only.</summary>
    [JsonProperty("related")]
    public string Related { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("professional")]
    public bool Professional { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("ranking")]
    public string Ranking { get; set; } = null!;

    [JsonProperty("country")]
    public string? Country { get; set; }

    [JsonProperty("language")]
    public string? Language { get; set; }

    [JsonProperty("about")]
    public string? About { get; set; }

    [JsonProperty("supporter")]
    public bool Supporter { get; set; }

    [JsonProperty("is_bot")]
    public bool IsBot { get; set; }

    [JsonProperty("bot_ai")]
    public string? BotAi { get; set; }

    [JsonProperty("bot_owner")]
    public int? BotOwner { get; set; }

    [JsonProperty("website")]
    public string? Website { get; set; }

    [JsonProperty("registration_date")]
    public DateTimeOffset? RegistrationDate { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("timeout_provisional")]
    public bool TimeoutProvisional { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("ratings")]
    public string Ratings { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("is_friend")]
    public string IsFriend { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("aga_id")]
    public string AgaId { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("ui_class")]
    public string UiClass { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("icon")]
    public string Icon { get; set; } = null!;
}

/// <summary>
/// A game challenge sent to a player.
/// </summary>
public class Challenge
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("challenger")]
    public string Challenger { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("challenged")]
    public string Challenged { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("game")]
    public string Game { get; set; } = null!;

    [JsonProperty("group")]
    public int? Group { get; set; }

    [JsonProperty("challenger_color")]
    public string? ChallengerColor { get; set; }

    [JsonProperty("aga_rated")]
    public bool? AgaRated { get; set; }
}

/// <summary>
/// A game record from a player's game list.
/// Decimal-valued fields (komi, ratings, handicap_rank_difference) are serialized as
/// strings by OGS; read-only fields (players, historical_ratings, flags…) are JSON blobs.
/// </summary>
public class Game
{
    /// <summary>Read-only.</summary>
    [JsonProperty("related")]
    public string Related { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("players")]
    public string Players { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("creator")]
    public int Creator { get; set; }

    [JsonProperty("mode")]
    public string? Mode { get; set; }

    [JsonProperty("source")]
    public string? Source { get; set; }

    [JsonProperty("black")]
    public int? Black { get; set; }

    [JsonProperty("white")]
    public int? White { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("rules")]
    public string? Rules { get; set; }

    [JsonProperty("ranked")]
    public bool? Ranked { get; set; }

    [JsonProperty("handicap_rank_difference")]
    public string? HandicapRankDifference { get; set; }

    [JsonProperty("handicap")]
    public int? Handicap { get; set; }

    [JsonProperty("komi")]
    public string? Komi { get; set; }

    [JsonProperty("time_control")]
    public string? TimeControl { get; set; }

    [JsonProperty("black_player_rank")]
    public int? BlackPlayerRank { get; set; }

    [JsonProperty("black_player_rating")]
    public string? BlackPlayerRating { get; set; }

    [JsonProperty("white_player_rank")]
    public int? WhitePlayerRank { get; set; }

    [JsonProperty("white_player_rating")]
    public string? WhitePlayerRating { get; set; }

    [JsonProperty("time_per_move")]
    public int? TimePerMove { get; set; }

    [JsonProperty("time_control_parameters")]
    public string? TimeControlParameters { get; set; }

    [JsonProperty("disable_analysis")]
    public bool? DisableAnalysis { get; set; }

    [JsonProperty("tournament")]
    public int? Tournament { get; set; }

    [JsonProperty("tournament_round")]
    public int? TournamentRound { get; set; }

    [JsonProperty("ladder")]
    public int? Ladder { get; set; }

    [JsonProperty("pause_on_weekends")]
    public bool? PauseOnWeekends { get; set; }

    [JsonProperty("outcome")]
    public string? Outcome { get; set; }

    [JsonProperty("black_lost")]
    public bool? BlackLost { get; set; }

    [JsonProperty("white_lost")]
    public bool? WhiteLost { get; set; }

    [JsonProperty("annulled")]
    public bool? Annulled { get; set; }

    [JsonProperty("started")]
    public DateTimeOffset? Started { get; set; }

    [JsonProperty("ended")]
    public DateTimeOffset? Ended { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("sgf_filename")]
    public string SgfFilename { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("historical_ratings")]
    public string HistoricalRatings { get; set; } = null!;

    [JsonProperty("rengo")]
    public bool? Rengo { get; set; }

    [JsonProperty("rengo_black_team")]
    public List<int>? RengoBlackTeam { get; set; }

    [JsonProperty("rengo_white_team")]
    public List<int>? RengoWhiteTeam { get; set; }

    [JsonProperty("rengo_casual_mode")]
    public bool? RengoCasualMode { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("flags")]
    public string Flags { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("bot_detection_results")]
    public string BotDetectionResults { get; set; } = null!;

    [JsonProperty("bot_parameters")]
    public object? BotParameters { get; set; }
}

/// <summary>
/// A group (club) a player belongs to. Read-only fields (admins, settings, founder…)
/// are serialized as JSON-encoded strings by OGS.
/// </summary>
public class PlayerGroup
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("short_description")]
    public string? ShortDescription { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("website")]
    public string? Website { get; set; }

    [JsonProperty("location")]
    public string? Location { get; set; }

    [JsonProperty("require_invitation")]
    public bool? RequireInvitation { get; set; }

    [JsonProperty("is_public")]
    public bool? IsPublic { get; set; }

    [JsonProperty("admin_only_tournaments")]
    public bool? AdminOnlyTournaments { get; set; }

    [JsonProperty("hide_details")]
    public bool? HideDetails { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("icon")]
    public string Icon { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("banner")]
    public string Banner { get; set; } = null!;

    [JsonProperty("has_banner")]
    public bool? HasBanner { get; set; }

    [JsonProperty("has_icon")]
    public bool? HasIcon { get; set; }

    [JsonProperty("member_count")]
    public int? MemberCount { get; set; }

    [JsonProperty("bulletin")]
    public string? Bulletin { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("admins")]
    public string Admins { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("latest_news")]
    public string LatestNews { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("settings")]
    public string Settings { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("is_member")]
    public string IsMember { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("ladder_ids")]
    public string LadderIds { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("founder")]
    public string Founder { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("rules")]
    public string Rules { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("handicap")]
    public string Handicap { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("has_tournament_records")]
    public string HasTournamentRecords { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("has_open_tournaments")]
    public string HasOpenTournaments { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("has_active_tournaments")]
    public string HasActiveTournaments { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("has_finished_tournaments")]
    public string HasFinishedTournaments { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("invitation_requests")]
    public string InvitationRequests { get; set; } = null!;
}
