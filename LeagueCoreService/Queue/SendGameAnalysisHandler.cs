using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Kifubara;
using Shared.Enum;

namespace LeagueCoreService.Queue;

public class SendGameAnalysisHandler(QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    KifubaraService kifubaraService)
    : ICommandHandler
{
    public string CommandType => "SendGameAnalysis";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var nextGame = await queueDataService.RunQueryAsync(
            new GetNextGameAnalysisQuery());
        
        var gameLink = await kifubaraService.SendSgf(nextGame.Sgf);

        await leagueDataService.ExecuteAsync(new SetMatchGameAnalysisUrlCommand()
        {
            MatchId = nextGame.MatchId,
            GameAnalysisUrl = gameLink
        });

        await queueDataService.ExecuteAsync(new SetGameAnalysisStatusCommand()
        {
            NewStatus = QueueStatus.Completed,
            UpdateProcessedTime = true,
            GameAnalysisId = nextGame.Id
        });
    }
}