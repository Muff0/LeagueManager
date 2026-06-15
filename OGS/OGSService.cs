using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OGS.Client;
using Shared;
using Shared.Dto.OGS;

namespace OGS;

public class OGSService(HttpClient httpClient,
    IOnlineLeagueClient ogsClient,
    IOgsPlayerClient playerClient,
                               ILogger<OGSService> logger) : ServiceBase(logger)
{
    private readonly string _baseAddress = "https://online-go.com";
    private readonly string gameUrlFormat = "https://online-go.com/game/";
    private readonly string leagueGameUrlFormat = "https://online-go.com/online-league/league-game/";
    
    private readonly double A = 525;
    private readonly double C = 23.15;
    private readonly double MAX_RATING = 6000;

    private readonly double MIN_RATING = 100;


    public double RatingToRank(double rating)
    {
        return Math.Log(Math.Min(MAX_RATING, Math.Max(MIN_RATING, rating)) / A) * C;
    }

    public async Task<OGSPlayer?> GetPlayer(string userId)
    {
        var url = _baseAddress + "/api/v1/players?username=" + userId;

        var res = await httpClient.GetAsync(url);
        var content = await res.Content.ReadAsStringAsync();

        var results = JsonConvert.DeserializeObject<GetPlayersResult>(content);

        if (results != null && results.Count == 1)
            return results.Results.First();
        return null;
    }

    public async Task Test()
    {
        var pl = await playerClient.GetPlayersAsync("muff0");
    }

    public async Task<string> GetSgf(string gameId)
    {
        return "";
    }

    private bool IsValidGameUrl(string url) => url.Contains(gameUrlFormat);
    private string GetIdFromGameUrl(string url) => url.Replace(gameUrlFormat, string.Empty);
    private string GetLeagueIdFromGameUrl(string url) => url.Replace(leagueGameUrlFormat, string.Empty);

    public async Task<string> GetMatchIdFromLeagueUrl(string matchMatchUrl)
    {
        if (IsValidGameUrl(matchMatchUrl))
            return GetIdFromGameUrl(matchMatchUrl);
        var id = GetIdFromGameUrl(matchMatchUrl);
        var intId = int.Parse(id);
        
        await ogsClient.GetMatchAsync(intId);
        
        return string.Empty;
    }
}