using System.Text;
using Newtonsoft.Json;
using OGS.Model;
using Shared;

namespace OGS.Client;

/// <summary>
/// HTTP client for the OGS online_league API.
///
/// Authentication is handled out-of-band by <see cref="OgsAuthenticatedHttpHandler"/>,
/// which attaches an OAuth2 client-credentials bearer token to every request.
///
/// URL structure: /api/v1/online_league/{auth_key}{resource}
/// The auth_key acts as a path prefix that namespaces all league-scoped endpoints.
/// It is the league's own key (returned by the leagues endpoint), not an auth credential,
/// so it is resolved lazily from <see cref="GetLeaguesAsync"/> and cached.
/// League-agnostic endpoints (leagues/, commence, match/{id}) omit the prefix.
/// </summary>
public class OnlineLeagueClient : IOnlineLeagueClient
{
    private readonly HttpClient _http;
    private readonly SemaphoreSlim _prefixLock = new(1, 1);
    private string? _authKeyPrefix;

    public OnlineLeagueClient(HttpClient http)
    {
        _http = http;
    }

    // -------------------------------------------------------------------------
    // Leagues
    // -------------------------------------------------------------------------

    public async Task<List<OnlineLeagueAdmin>> GetLeaguesAsync(CancellationToken ct = default)
        => await GetAsync<List<OnlineLeagueAdmin>>("online_league/leagues/", ct);

    public async Task<OnlineLeagueAdmin> CreateLeagueAsync(OnlineLeagueAdmin league, CancellationToken ct = default)
        => await PostAsync<OnlineLeagueAdmin, OnlineLeagueAdmin>("online_league/leagues/", league, ct);

    public async Task DeleteLeagueAsync(CancellationToken ct = default)
        => await DeleteAsync("online_league/leagues/", ct);

    // -------------------------------------------------------------------------
    // Callback
    // -------------------------------------------------------------------------

    public async Task<OnlineLeagueCallback> GetCallbackAsync(CancellationToken ct = default)
        => await GetAsync<OnlineLeagueCallback>(await LeaguePath("callback", ct), ct);

    public async Task<OnlineLeagueCallback> UpdateCallbackAsync(OnlineLeagueCallback callback, CancellationToken ct = default)
        => await PutAsync<OnlineLeagueCallback, OnlineLeagueCallback>(await LeaguePath("callback", ct), callback, ct);

    public async Task<OnlineLeagueCallback> PatchCallbackAsync(OnlineLeagueCallback callback, CancellationToken ct = default)
        => await PatchAsync<OnlineLeagueCallback, OnlineLeagueCallback>(await LeaguePath("callback", ct), callback, ct);

    // -------------------------------------------------------------------------
    // Matches (admin)
    // -------------------------------------------------------------------------

    public async Task<PaginatedList<OnlineLeagueMatchAdmin>> GetMatchesAsync(
        int? page = null,
        int? pageSize = null,
        string? ordering = null,
        CancellationToken ct = default)
    {
        var query = BuildQuery(
            ("page", page?.ToString()),
            ("page_size", pageSize?.ToString()),
            ("ordering", ordering));

        return await GetAsync<PaginatedList<OnlineLeagueMatchAdmin>>(await LeaguePath("matches/", ct) + query, ct);
    }

    public async Task<OnlineLeagueMatchAdmin> CreateMatchAsync(OnlineLeagueMatchAdmin match, CancellationToken ct = default)
        => await PostAsync<OnlineLeagueMatchAdmin, OnlineLeagueMatchAdmin>(await LeaguePath("matches/", ct), match, ct);

    public async Task<OnlineLeagueMatchAdmin> UpdateMatchAsync(OnlineLeagueMatchAdmin match, CancellationToken ct = default)
        => await PutAsync<OnlineLeagueMatchAdmin, OnlineLeagueMatchAdmin>(await LeaguePath("matches/", ct), match, ct);

    public async Task DeleteMatchesAsync(CancellationToken ct = default)
        => await DeleteAsync(await LeaguePath("matches/", ct), ct);

    public async Task GetMatchAsync(int matchId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync(await LeaguePath($"matches/{matchId}", ct), ct);
        response.EnsureSuccessStatusCode();
    }

    // -------------------------------------------------------------------------
    // Members
    // -------------------------------------------------------------------------

    public async Task<OnlineLeagueMember> GetMemberAsync(string memberId, CancellationToken ct = default)
        => await GetAsync<OnlineLeagueMember>(await LeaguePath($"member/{memberId}", ct), ct);

    public async Task<OnlineLeagueMember> UpdateMemberAsync(string memberId, OnlineLeagueMember member, CancellationToken ct = default)
        => await PutAsync<OnlineLeagueMember, OnlineLeagueMember>(await LeaguePath($"member/{memberId}", ct), member, ct);

    public async Task DeleteMemberAsync(string memberId, CancellationToken ct = default)
        => await DeleteAsync(await LeaguePath($"member/{memberId}", ct), ct);

    // -------------------------------------------------------------------------
    // Commence
    // -------------------------------------------------------------------------

    public async Task CommenceAsync(CancellationToken ct = default)
    {
        var response = await _http.PutAsync("online_league/commence", null, ct);
        response.EnsureSuccessStatusCode();
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    /// <summary>
    /// Builds a path scoped to this league's auth_key prefix.
    /// e.g. LeaguePath("matches/") → "online_league/ABC123matches/"
    /// </summary>
    private async Task<string> LeaguePath(string resource, CancellationToken ct)
        => $"online_league/{await GetAuthKeyPrefixAsync(ct)}{resource}";

    /// <summary>
    /// Resolves and caches this league's auth_key, which prefixes every league-scoped path.
    /// The key is fetched from the leagues endpoint, which is authenticated by the bearer
    /// token but not itself league-scoped. Assumes the credentials map to a single league.
    /// </summary>
    private async Task<string> GetAuthKeyPrefixAsync(CancellationToken ct)
    {
        if (_authKeyPrefix is not null)
            return _authKeyPrefix;

        await _prefixLock.WaitAsync(ct);
        try
        {
            if (_authKeyPrefix is not null)
                return _authKeyPrefix;

            var leagues = await GetLeaguesAsync(ct);
            if (leagues.Count != 1)
                throw new InvalidOperationException(
                    $"Expected the OGS credentials to map to exactly one online league, but found {leagues.Count}.");

            return _authKeyPrefix = leagues[0].AuthKey;
        }
        finally
        {
            _prefixLock.Release();
        }
    }

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

    private async Task<TResponse> PatchAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct)
    {
        var response = await _http.PatchAsync(path, Serialize(body), ct);
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