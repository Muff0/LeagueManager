using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OGS.Client;
using Shared;
using Shared.Dto.OGS;

namespace OGS;

public class OgsService(HttpClient httpClient,
    IOgsPlayerClient playerClient,
    IUnauthenticatedOgsClient ogsClient,
                               ILogger<OgsService> logger) : ServiceBase(logger)
{
    private readonly string _baseAddress = "https://online-go.com";
    private readonly string _gameUrlFormat = "https://online-go.com/game/";
    private readonly string _leagueGameUrlFormat = "https://online-go.com/online-league/league-game/";
    
    private readonly double _a = 525;
    private readonly double _c = 23.15;
    private readonly double _maxRating = 6000;

    private readonly double _minRating = 100;


    public double RatingToRank(double rating)
    {
        return Math.Log(Math.Min(_maxRating, Math.Max(_minRating, rating)) / _a) * _c;
    }
    
    public async Task<OgsPlayer?> GetPlayer(string userId)
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

    public async Task<string> GetSgf(int gameId)
    {
        var sgf = await ogsClient.GetSgfAsync(gameId);
        return sgf;
    }

    private bool IsValidGameUrl(string url) => url.Contains(_gameUrlFormat);
    private string GetIdFromGameUrl(string url) => url.Replace(_gameUrlFormat, string.Empty);
    private string GetLeagueIdFromGameUrl(string url) => url.Replace(_leagueGameUrlFormat, string.Empty);

    public async Task<int> GetMatchIdFromLeagueId(int matchId)
    {
        
        var match = await ogsClient.GetOnlineLeagueMatchInfoAsync(matchId);
        
        return match.Game ?? 0;
    }
}