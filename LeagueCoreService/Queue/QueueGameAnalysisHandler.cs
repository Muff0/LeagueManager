using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using OGS;
using OGS.Model;
using Shared.Dto;

namespace LeagueCoreService.Queue;

public class QueueGameAnalysisHandler(LeagueDataService leagueDataService,
    QueueDataService queueDataService,
    OGSService ogsService)
    : ICommandHandler
{
    public string CommandType => "QueueGameAnalysis";

    public async Task HandleAsync(CommandMessage cmd)
    {
        var season = await leagueDataService.RunQueryAsync(new GetActiveSeasonQuery());
        var round = 4;

        var matchesToSchedule = await leagueDataService.RunQueryAsync(new GetMatchesQuery()
        {
            HasGameAnalysisUrl = false,
            HasScheduledGameAnalysis = false,
            SeasonId = season!.Id,
            Round = round
        });

        var queued = new List<MatchDto>();
        foreach (var match in matchesToSchedule)
        {
            var sgf = await ogsService.GetMatchIdFromLeagueUrl(match.MatchUrl);
            if (IsValidSgfFormat(sgf))
            {
                var gameAnalysis = new GameAnalysis()
                {
                    MatchId =  match.Id,
                    Sgf = sgf
                };

                await queueDataService.ExecuteAsync(new InsertGameAnalysisCommand()
                {
                    NewGameAnalysis = gameAnalysis
                });
                
                queued.Add(match.ToMatchDto());
            }
        }
    }
    
    private bool IsValidSgfFormat(string sgf)
    {
        //For now let's just check if there's anything inside
        return !sgf.IsWhiteSpace();
    }
}