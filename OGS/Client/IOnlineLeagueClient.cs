using OGS.Model;

namespace OGS.Client;

public interface IOnlineLeagueClient
{
    // -------------------------------------------------------------------------
    // Leagues
    // -------------------------------------------------------------------------

    Task<List<OnlineLeagueAdmin>> GetLeaguesAsync(CancellationToken ct = default);
    Task<OnlineLeagueAdmin> CreateLeagueAsync(OnlineLeagueAdmin league, CancellationToken ct = default);
    Task DeleteLeagueAsync(CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Callback
    // -------------------------------------------------------------------------

    Task<OnlineLeagueCallback> GetCallbackAsync(CancellationToken ct = default);
    Task<OnlineLeagueCallback> UpdateCallbackAsync(OnlineLeagueCallback callback, CancellationToken ct = default);
    Task<OnlineLeagueCallback> PatchCallbackAsync(OnlineLeagueCallback callback, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Matches (admin view — includes invite links, game settings, ratings)
    // -------------------------------------------------------------------------

    Task<PaginatedList<OnlineLeagueMatchAdmin>> GetMatchesAsync(
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default);

    Task<OnlineLeagueMatchAdmin> CreateMatchAsync(OnlineLeagueMatchAdmin match, CancellationToken ct = default);
    Task<OnlineLeagueMatchAdmin> UpdateMatchAsync(OnlineLeagueMatchAdmin match, CancellationToken ct = default);
    Task DeleteMatchesAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves a specific match by OGS match ID.
    /// Note: OGS returns no response body for this endpoint.
    /// </summary>
    Task GetMatchAsync(int matchId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Match info (public view — no invite links or ratings)
    // -------------------------------------------------------------------------

    Task<OnlineLeagueMatchInfo> GetMatchInfoAsync(int id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Members
    // -------------------------------------------------------------------------

    Task<OnlineLeagueMember> GetMemberAsync(string memberId, CancellationToken ct = default);
    Task<OnlineLeagueMember> UpdateMemberAsync(string memberId, OnlineLeagueMember member, CancellationToken ct = default);
    Task DeleteMemberAsync(string memberId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Commence
    // -------------------------------------------------------------------------

    Task CommenceAsync(CancellationToken ct = default);
}