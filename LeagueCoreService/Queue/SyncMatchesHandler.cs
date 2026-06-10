using Data;
using Data.Commands.Match;
using Data.Model;
using Data.Queries;
using LeagoService;
using Shared.Dto;

namespace LeagueCoreService.Queue;

public class SyncMatchesHandler(
    LeagueDataService leagueDataService,
    LeagoMainService leagoService)
    : ICommandHandler
{
    public string CommandType => "SyncMatches";

    public async Task HandleAsync(CommandMessage cmd)
    {
        await SyncMatches();
    }


    public async Task SyncMatchesForRound(int round)
    {
        var activeSeason = await leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
        if (activeSeason == null)
            return;

        var getMatchesRes = await leagoService.GetMatches(new GetMatchesInDto
        {
            RoundKey = round,
            TournamentKey = activeSeason.LeagoL2Key,
            MatchesCount = 100
        });

        if (getMatchesRes != null)
            await leagueDataService.ExecuteAsync(
                new AddOrUpdateMatchesCommand
                {
                    ToUpdate = getMatchesRes.Matches,
                    SeasonId = activeSeason.Id,
                    Round = round
                });
    }

    private async Task SyncMatches()
    {
        for (var ii = 0; ii < 5; ii++)
            await SyncMatchesForRound(ii + 1);
    }
}