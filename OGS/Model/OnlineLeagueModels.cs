using Newtonsoft.Json;

namespace OGS.Model;

public class OnlineLeagueAdmin
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("auth_key")]
    public string AuthKey { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("member_count")]
    public string MemberCount { get; set; } = null!;
}

public class OnlineLeagueCallback
{
    [JsonProperty("callback_url_template")]
    public string? CallbackUrlTemplate { get; set; }
}

public class OnlineLeagueMatchAdmin
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    /// <summary>Your internal match identifier — used to correlate OGS matches back to LeagueManager.</summary>
    [JsonProperty("league_match_id")]
    public string? LeagueMatchId { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("black_member_id")]
    public string BlackMemberId { get; set; } = null!;

    /// <summary>Read-only. Invite link sent to the black player.</summary>
    [JsonProperty("black_invite")]
    public string BlackInvite { get; set; } = null!;

    [JsonProperty("white_member_id")]
    public string WhiteMemberId { get; set; } = null!;

    /// <summary>Read-only. Invite link sent to the white player.</summary>
    [JsonProperty("white_invite")]
    public string WhiteInvite { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("spectator_link")]
    public string SpectatorLink { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("league")]
    public string League { get; set; } = null!;

    [JsonProperty("rules")]
    public string? Rules { get; set; }

    [JsonProperty("handicap")]
    public int? Handicap { get; set; }

    [JsonProperty("height")]
    public int? Height { get; set; }

    [JsonProperty("width")]
    public int? Width { get; set; }

    [JsonProperty("time_control")]
    public string? TimeControl { get; set; }

    [JsonProperty("main_time")]
    public int? MainTime { get; set; }

    [JsonProperty("period_time")]
    public int? PeriodTime { get; set; }

    [JsonProperty("periods")]
    public int? Periods { get; set; }

    [JsonProperty("stones_per_period")]
    public int? StonesPerPeriod { get; set; }

    [JsonProperty("time_increment")]
    public int? TimeIncrement { get; set; }

    [JsonProperty("initial_time")]
    public int? InitialTime { get; set; }

    [JsonProperty("max_time")]
    public int? MaxTime { get; set; }

    [JsonProperty("per_move")]
    public int? PerMove { get; set; }

    [JsonProperty("total_time")]
    public int? TotalTime { get; set; }

    /// <summary>OGS game ID, populated once both players have accepted their invites.</summary>
    [JsonProperty("game")]
    public int? Game { get; set; }

    [JsonProperty("started")]
    public bool Started { get; set; }

    [JsonProperty("finished")]
    public bool Finished { get; set; }

    [JsonProperty("cancelled")]
    public bool Cancelled { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("outcome")]
    public string Outcome { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("black_lost")]
    public string BlackLost { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("white_lost")]
    public string WhiteLost { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("annulled")]
    public string Annulled { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("moderator_annulled")]
    public string ModeratorAnnulled { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("annulment_reason")]
    public string AnnulmentReason { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("rating_complete")]
    public string RatingComplete { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("black_member_rating")]
    public string BlackMemberRating { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("white_member_rating")]
    public string WhiteMemberRating { get; set; } = null!;
}

public class OnlineLeagueMatchInfo
{
    /// <summary>Read-only.</summary>
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("spectator_link")]
    public string SpectatorLink { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("league")]
    public string League { get; set; } = null!;

    [JsonProperty("game")]
    public int? Game { get; set; }

    [JsonProperty("started")]
    public bool Started { get; set; }

    [JsonProperty("finished")]
    public bool Finished { get; set; }

    [JsonProperty("cancelled")]
    public bool Cancelled { get; set; }

    [JsonProperty("black_ready")]
    public bool BlackReady { get; set; }

    [JsonProperty("white_ready")]
    public bool WhiteReady { get; set; }

    /// <summary>Read-only.</summary>
    [JsonProperty("outcome")]
    public string Outcome { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("black_lost")]
    public string BlackLost { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("white_lost")]
    public string WhiteLost { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("annulled")]
    public string Annulled { get; set; } = null!;
}

public class OnlineLeagueMember
{
    [JsonProperty("membership_id")]
    public string MembershipId { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("league_rating")]
    public string LeagueRating { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("ogs_player")]
    public string OgsPlayer { get; set; } = null!;

    /// <summary>Read-only.</summary>
    [JsonProperty("pending_rating_change")]
    public string PendingRatingChange { get; set; } = null!;
}

public class PaginatedList<T>
{
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("next")]
    public string? Next { get; set; }

    [JsonProperty("previous")]
    public string? Previous { get; set; }

    [JsonProperty("results")]
    public List<T> Results { get; set; } = [];
}