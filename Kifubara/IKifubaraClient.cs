using Kifubara.Models;

namespace Kifubara;

public interface IKifubaraClient
{
    /// <summary>
    /// Submits an SGF for KataGo analysis. Idempotent — the same SGF returns the existing game
    /// without triggering re-analysis. Check <see cref="AnalyzeGameResponse.Deduped"/> to distinguish.
    /// </summary>
    /// <exception cref="HttpRequestException">400 (bad SGF), 413 (too large >256 KB), 401 (unauthorized).</exception>
    Task<AnalyzeGameResponse> AnalyzeAsync(AnalyzeGameRequest request, CancellationToken ct = default);

    /// <summary>
    /// Gets the analysis state for a single game. Poll until <see cref="GameStateResponse.IsComplete"/>.
    /// Analysis typically takes a few minutes.
    /// </summary>
    /// <exception cref="HttpRequestException">404 if the game ID is unknown.</exception>
    Task<GameStateResponse> GetGameStateAsync(string gameId, CancellationToken ct = default);

    /// <summary>
    /// Gets all your games, newest first. All query parameters are optional.
    /// </summary>
    Task<GamesListResponse> GetGamesAsync(GetGamesQuery? query = null, CancellationToken ct = default);

    /// <summary>
    /// Deletes a game and its analysis. Intended for test cleanup.
    /// </summary>
    /// <exception cref="HttpRequestException">404 if the game ID is unknown.</exception>
    Task<DeleteGameResponse> DeleteGameAsync(string gameId, CancellationToken ct = default);
}