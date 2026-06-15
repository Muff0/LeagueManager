using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using OGS;
using OGS.Model;
using Shared.Dto;
using Shared.Enum;

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
        var round = 3;

        var matchesToSchedule = await leagueDataService.RunQueryAsync(new GetMatchesQuery()
        {
            HasGameAnalysisUrl = false,
            HasScheduledGameAnalysis = false,
            IsPlayed = true,
            SeasonId = season!.Id,
            Round = round
        });

        var queued = new List<MatchDto>();
        foreach (var match in matchesToSchedule)
        {
            var sgf = "";
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

        leagueDataService.ExecuteAsync(new SetMatchesGameAnalysisStatusCommand()
        {
            NewStatus = GameAnalysisStatus.Queued,
            Matches = queued.ToArray()
        });
    }
    
    private bool IsValidSgfFormat(string sgf)
    {
        //For now let's just check if there's anything inside
        return !sgf.IsWhiteSpace();
    }
}