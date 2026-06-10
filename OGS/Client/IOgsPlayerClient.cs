using OGS.Model;

namespace OGS.Client;

public interface IOgsPlayerClient
{
    // -------------------------------------------------------------------------
    // Players
    // -------------------------------------------------------------------------

    /// <summary>
    /// Lists and searches players. All filters are optional; omit to page through all players.
    /// </summary>
    Task<PaginatedList<MinimalPlayer>> GetPlayersAsync(
        string? username = null,
        string? country = null,
        string? language = null,
        bool? isBot = null,
        bool? isModerator = null,
        bool? professional = null,
        string? ordering = null,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default);

    Task<Player> GetPlayerAsync(int id, CancellationToken ct = default);
    Task<Player> UpdatePlayerAsync(int id, Player player, CancellationToken ct = default);
    Task DeletePlayerAsync(int id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Challenge
    // -------------------------------------------------------------------------

    /// <summary>Sends a game challenge to a specific player.</summary>
    Task<Challenge> ChallengePlayerAsync(int id, Challenge challenge, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Games
    // -------------------------------------------------------------------------

    Task<PaginatedList<Game>> GetPlayerGamesAsync(
        int id,
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Groups
    // -------------------------------------------------------------------------

    Task<PaginatedList<PlayerGroup>> GetPlayerGroupsAsync(
        int id,
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default);
}
