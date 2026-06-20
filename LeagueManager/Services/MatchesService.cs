using Data;
using Data.Queries;
using LeagueManager.ViewModel;
using Shared;
using Shared.Dto;

namespace LeagueManager.Services;

public class MatchesService(LeagueDataService leagueDataService,
    ILogger<MatchesService> logger) : ServiceBase(logger)
{
    public async Task<MatchDto[]> GetUnplayedMatches(int round)
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(
            new GetActiveSeasonQuery());
        
        var resGetm = await leagueDataService.RunQueryAsync(
            new GetMatchesQuery()
            {
                WithSeasonId = activeSeason!.Id,
                IsPlayed = false,
            });

        return resGetm.Select(mm => mm.ToMatchDto()).ToArray();
    }

    public async Task ProcessForfeitsAsync(List<UnplayedMatchViewModel> matchesWithForfeits)
    {
        throw new NotImplementedException();
    }

    public async Task<List<UnplayedMatchViewModel>> GetUnplayedMatchesAsync(int round, int? startIndex = null, int? count= null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            HandleException(e);
        }

        return new List<UnplayedMatchViewModel>();
    }

    public async Task<List<int>> GetAvailableRoundsAsync()
    {
        return [1, 2, 3, 4, 5];
    }
}