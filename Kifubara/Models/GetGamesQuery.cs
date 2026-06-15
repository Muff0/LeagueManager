namespace Kifubara.Models;

/// <summary>
/// Optional filters and pagination for GET /api/gomagic/games.
/// All properties are optional — omit any to skip that filter.
/// </summary>
public class GetGamesQuery
{
    /// <summary>
    /// Filter by analysis state. Use constants from <see cref="GameAnalysisState"/>.
    /// <see cref="GameAnalysisState.Pending"/> matches all not-yet-done games (queued + analyzing).
    /// </summary>
    public string? State { get; set; }

    public string? MatchId { get; set; }
    public string? League { get; set; }
    public string? Season { get; set; }
    public string? Round { get; set; }

    /// <summary>Page size. Default 100, max 500.</summary>
    public int? Limit { get; set; }

    /// <summary>Number of items to skip.</summary>
    public int? Offset { get; set; }
}

/// <summary>State string constants for use with <see cref="GetGamesQuery.State"/>.</summary>
public static class GameAnalysisState
{
    public const string Queued    = "queued";
    public const string Analyzing = "analyzing";
    public const string Done      = "done";
    public const string Error     = "error";
    /// <summary>Filter-only pseudo-state: matches all games not yet done (queued + analyzing).</summary>
    public const string Pending   = "pending";
}