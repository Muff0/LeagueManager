using Data;
using Data.Queries;
using LeagueManager.ViewModel;
using Shared.Dto;

namespace LeagueManager.Services;

public class MatchesService(LeagueDataService leagueDataService)
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

    public async Task<List<UnplayedMatchViewModel>?> GetUnplayedMatchesAsync(int selectedRound)
    {
        throw new NotImplementedException();
    }

    public async Task<List<int>> GetAvailableRoundsAsync()
    {
        throw new NotImplementedException();
    }
}