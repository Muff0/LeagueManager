using System.Text;
using Newtonsoft.Json;
using OGS.Model;

namespace OGS.Client;

public class GamesClient : IGamesClient
{
    private readonly HttpClient _http;

    public GamesClient(HttpClient http)
    {
        _http = http;
    }

    // -------------------------------------------------------------------------
    // Game
    // -------------------------------------------------------------------------

    public async Task<GameDetail> GetGameAsync(int id, CancellationToken ct = default)
        => await GetAsync<GameDetail>($"games/{id}/", ct);

    public async Task DeleteGameAsync(int id, CancellationToken ct = default)
        => await DeleteAsync($"games/{id}/", ct);

    // -------------------------------------------------------------------------
    // Actions
    // -------------------------------------------------------------------------

    public async Task SubmitMoveAsync(int id, object body, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"games/{id}/move/", Serialize(body), ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task PassAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"games/{id}/pass/", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task PauseAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"games/{id}/pause/", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task ResumeAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"games/{id}/resume/", null, ct);
        response.EnsureSuccessStatusCode();
    }

    // -------------------------------------------------------------------------
    // Navigation
    // -------------------------------------------------------------------------

    public async Task GetNextGameAsync(int id, int userId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"games/{id}/next/{userId}", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task GetPrevGameAsync(int id, int userId, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"games/{id}/prev/{userId}", ct);
        response.EnsureSuccessStatusCode();
    }

    // -------------------------------------------------------------------------
    // ACL
    // -------------------------------------------------------------------------

    public async Task GetAclAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.GetAsync($"games/{id}/acl/", ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateAclEntryAsync(int id, CancellationToken ct = default)
    {
        var response = await _http.PostAsync($"games/{id}/acl/", null, ct);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAclEntryAsync(int aclEntryId, CancellationToken ct = default)
        => await DeleteAsync($"games/acl/{aclEntryId}/", ct);

    // -------------------------------------------------------------------------
    // Queries
    // -------------------------------------------------------------------------

    public async Task BetweenPlayersSinceAsync(object body, CancellationToken ct = default)
    {
        var response = await _http.PostAsync("games/between_players_since/", Serialize(body), ct);
        response.EnsureSuccessStatusCode();
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

    private async Task DeleteAsync(string path, CancellationToken ct)
    {
        var response = await _http.DeleteAsync(path, ct);
        response.EnsureSuccessStatusCode();
    }

    private static StringContent Serialize(object obj)
        => new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
}