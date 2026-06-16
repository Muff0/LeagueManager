using Newtonsoft.Json;
using OGS.Model;

namespace OGS.Client;

public class UnauthenticatedOgsClient : IUnauthenticatedOgsClient
{
    
    private readonly HttpClient _http;

    public UnauthenticatedOgsClient(HttpClient http)
    {
        _http = http;
    }
    // -------------------------------------------------------------------------
    // Game state / SGF
    // -------------------------------------------------------------------------

    public async Task<string> GetGameStateAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"games/{id}/state/", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    public async Task<string> GetSgfAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"games/{id}/sgf/", ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    // -------------------------------------------------------------------------
    // Online League match info (public)
    // -------------------------------------------------------------------------

    public async Task<OnlineLeagueMatchInfo> GetOnlineLeagueMatchInfoAsync(int id, CancellationToken ct = default)
        => await GetAsync<OnlineLeagueMatchInfo>($"online_league/match/{id}", ct);

    
    private async Task<T> GetAsync<T>(string path, CancellationToken ct)
    {
        var response = await _http.GetAsync(path, ct);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<T>(json)!;
    }
}