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
}