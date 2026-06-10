using System.Text;
using Newtonsoft.Json;
using OGS.Model;

namespace OGS.Client;

/// <summary>
/// HTTP client for the OGS players API.
///
/// Authentication is handled out-of-band by <see cref="OgsAuthenticatedHttpHandler"/>,
/// which attaches an OAuth2 client-credentials bearer token to every request.
///
/// All endpoints live under /api/v1/players/ and are not league-scoped, so — unlike
/// <see cref="OnlineLeagueClient"/> — no auth_key path prefix is involved.
/// </summary>
public class OgsPlayerClient : IOgsPlayerClient
{
    private readonly HttpClient _http;

    public OgsPlayerClient(HttpClient http)
    {
        _http = http;
    }

    // -------------------------------------------------------------------------
    // Players
    // -------------------------------------------------------------------------

    public async Task<PaginatedList<MinimalPlayer>> GetPlayersAsync(
        string? username = null,
        string? country = null,
        string? language = null,
        bool? isBot = null,
        bool? isModerator = null,
        bool? professional = null,
        string? ordering = null,
        int? page = null,
        int? pageSize = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(
            ("username", username),
            ("country", country),
            ("language", language),
            ("is_bot", isBot?.ToString().ToLowerInvariant()),
            ("is_moderator", isModerator?.ToString().ToLowerInvariant()),
            ("professional", professional?.ToString().ToLowerInvariant()),
            ("ordering", ordering),
            ("page", page?.ToString()),
            ("page_size", pageSize?.ToString()));

        return await GetAsync<PaginatedList<MinimalPlayer>>("players/" + query, ct);
    }

    public async Task<Player> GetPlayerAsync(int id, CancellationToken ct = default)
        => await GetAsync<Player>($"players/{id}/", ct);

    public async Task<Player> UpdatePlayerAsync(int id, Player player, CancellationToken ct = default)
        => await PutAsync<Player, Player>($"players/{id}/", player, ct);

    public async Task DeletePlayerAsync(int id, CancellationToken ct = default)
        => await DeleteAsync($"players/{id}/", ct);

    // -------------------------------------------------------------------------
    // Challenge
    // -------------------------------------------------------------------------

    public async Task<Challenge> ChallengePlayerAsync(int id, Challenge challenge, CancellationToken ct = default)
        => await PostAsync<Challenge, Challenge>($"players/{id}/challenge/", challenge, ct);

    // -------------------------------------------------------------------------
    // Games
    // -------------------------------------------------------------------------

    public async Task<PaginatedList<Game>> GetPlayerGamesAsync(
        int id,
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(
            ("page", page?.ToString()),
            ("page_size", pageSize?.ToString()),
            ("ordering", ordering));

        return await GetAsync<PaginatedList<Game>>($"players/{id}/games/" + query, ct);
    }

    // -------------------------------------------------------------------------
    // Groups
    // -------------------------------------------------------------------------

    public async Task<PaginatedList<PlayerGroup>> GetPlayerGroupsAsync(
        int id,
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(
            ("page", page?.ToString()),
            ("page_size", pageSize?.ToString()),
            ("ordering", ordering));

        return await GetAsync<PaginatedList<PlayerGroup>>($"players/{id}/groups/" + query, ct);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<T> GetAsync<T>(string path, CancellationToken ct)
    {
        var response = await _http.GetAsync(path, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<T>(json)!;
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct)
    {
        var response = await _http.PostAsync(path, Serialize(body), ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<TResponse>(json)!;
    }

    private async Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct)
    {
        var response = await _http.PutAsync(path, Serialize(body), ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<TResponse>(json)!;
    }

    private async Task DeleteAsync(string path, CancellationToken ct)
    {
        var response = await _http.DeleteAsync(path, ct);
        response.EnsureSuccessStatusCode();
    }

    private static StringContent Serialize<T>(T obj)
        => new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

    private static string BuildQuery(params (string key, string? value)[] parameters)
    {
        var pairs = parameters
            .Where(p => p.value is not null)
            .Select(p => $"{p.key}={Uri.EscapeDataString(p.value!)}");

        var query = string.Join("&", pairs);
        return query.Length > 0 ? "?" + query : string.Empty;
    }
}
