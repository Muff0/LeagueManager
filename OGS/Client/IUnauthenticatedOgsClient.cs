using OGS.Model;

namespace OGS.Client;

public interface IUnauthenticatedOgsClient
{
    
    // -------------------------------------------------------------------------
    // League match info (public view — no invite links or ratings)
    // -------------------------------------------------------------------------

    Task<OnlineLeagueMatchInfo> GetOnlineLeagueMatchInfoAsync(int id, CancellationToken ct = default);
    
    // -------------------------------------------------------------------------
    // Game state / SGF
    // -------------------------------------------------------------------------

    /// <summary>Returns the raw JSON board state for the game.</summary>
    Task<string> GetGameStateAsync(int id, CancellationToken ct = default);


    /// <summary>Returns the SGF file content as a string.</summary>
    Task<string> GetSgfAsync(int id, CancellationToken ct = default);
}