using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Kifubara;
using Shared.Enum;

namespace LeagueCoreService.Queue;

public class StartGameAnalysisHandler(QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    KifubaraService kifubaraService)
    : ICommandHandler
{
    public string CommandType => "StartGameAnalysis";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var nextGame = await queueDataService.TakeFirstAsync(
            new GetGameAnalysisQuery()
            {
                OrderByCreatedAt = true,
                WithStatus = QueueStatus.Pending
            });
        
        if (nextGame == null)
            return;
        
        var resA = await kifubaraService.SendSgf(nextGame.Sgf);

        await leagueDataService.ExecuteAsync(new UpdateMatchGameAnalysisCommand()
        {
            MatchId = nextGame.MatchId,
            SetNewStatus = GameAnalysisStatus.InProgress,
            GameAnalysisUrl = resA.ShareUrl,
        });

        await queueDataService.ExecuteAsync(new UpdateGameAnalysisCommand()
        {
            NewStatus = QueueStatus.Processing,
            UpdateProcessedTime = true,
            GameAnalysisId = nextGame.Id,
            StateUrl = resA.StateUrl
        });
    }
}