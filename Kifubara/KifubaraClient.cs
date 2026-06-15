using System.Text;
using Kifubara.Models;
using Newtonsoft.Json;

namespace Kifubara;

public class KifubaraClient(HttpClient httpClient) : IKifubaraClient
{
    private static readonly JsonSerializerSettings SerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore
    };

    public async Task<AnalyzeGameResponse> AnalyzeAsync(AnalyzeGameRequest request, CancellationToken ct = default)
    {
        var json = JsonConvert.SerializeObject(request, SerializerSettings);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("api/gomagic/analyze", content, ct);
        response.EnsureSuccessStatusCode(); // 200 (deduped) and 201 (new) both pass
        return await ReadJsonAsync<AnalyzeGameResponse>(response, ct);
    }

    public async Task<GameStateResponse> GetGameStateAsync(string gameId, CancellationToken ct = default)
    {
        var response = await httpClient.GetAsync($"api/gomagic/games/{Uri.EscapeDataString(gameId)}/state", ct);
        response.EnsureSuccessStatusCode();
        return await ReadJsonAsync<GameStateResponse>(response, ct);
    }

    public async Task<GamesListResponse> GetGamesAsync(GetGamesQuery? query = null, CancellationToken ct = default)
    {
        var url = BuildGamesUrl(query ?? new GetGamesQuery());
        var response = await httpClient.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();
        return await ReadJsonAsync<GamesListResponse>(response, ct);
    }

    public async Task<DeleteGameResponse> DeleteGameAsync(string gameId, CancellationToken ct = default)
    {
        var response = await httpClient.DeleteAsync($"api/gomagic/games/{Uri.EscapeDataString(gameId)}", ct);
        response.EnsureSuccessStatusCode();
        return await ReadJsonAsync<DeleteGameResponse>(response, ct);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static async Task<T> ReadJsonAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        var body = await response.Content.ReadAsStringAsync(ct);
        return JsonConvert.DeserializeObject<T>(body)!;
    }

    private static string BuildGamesUrl(GetGamesQuery query)
    {
        var sb = new StringBuilder("api/gomagic/games");
        var first = true;

        void Append(string key, string? value)
        {
            if (value is null) return;
            sb.Append(first ? '?' : '&');
            sb.Append(Uri.EscapeDataString(key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(value));
            first = false;
        }

        Append("state",    query.State);
        Append("match_id", query.MatchId);
        Append("league",   query.League);
        Append("season",   query.Season);
        Append("round",    query.Round);
        if (query.Limit.HasValue)  Append("limit",  query.Limit.Value.ToString());
        if (query.Offset.HasValue) Append("offset", query.Offset.Value.ToString());

        return sb.ToString();
    }
}