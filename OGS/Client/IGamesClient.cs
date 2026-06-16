using OGS.Model;

namespace OGS.Client;

public interface IGamesClient
{
    // -------------------------------------------------------------------------
    // Game
    // -------------------------------------------------------------------------

    Task<GameDetail> GetGameAsync(int id, CancellationToken ct = default);
    Task DeleteGameAsync(int id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Actions (move, pass, pause, resume)
    // -------------------------------------------------------------------------

    /// <summary>Submit a move. Body shape is not defined by OGS — pass a raw object.</summary>
    Task SubmitMoveAsync(int id, object body, CancellationToken ct = default);

    Task PassAsync(int id, CancellationToken ct = default);
    Task PauseAsync(int id, CancellationToken ct = default);
    Task ResumeAsync(int id, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Navigation
    // -------------------------------------------------------------------------

    Task GetNextGameAsync(int id, int userId, CancellationToken ct = default);
    Task GetPrevGameAsync(int id, int userId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // ACL (private games)
    // -------------------------------------------------------------------------

    Task GetAclAsync(int id, CancellationToken ct = default);
    Task CreateAclEntryAsync(int id, CancellationToken ct = default);
    Task DeleteAclEntryAsync(int aclEntryId, CancellationToken ct = default);

    // -------------------------------------------------------------------------
    // Queries
    // -------------------------------------------------------------------------

    /// <summary>
    /// Find games between a set of players since a given date.
    /// Body shape is not defined by OGS — pass a raw object.
    /// </summary>
    Task BetweenPlayersSinceAsync(object body, CancellationToken ct = default);
}