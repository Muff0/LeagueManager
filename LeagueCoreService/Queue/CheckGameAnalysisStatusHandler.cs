using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Kifubara.Models;
using Mail;
using Shared.Enum;

namespace LeagueCoreService.Queue;

public class CheckGameAnalysisStatusHandler
(QueueDataService queueDataService,
    LeagueDataService leagueDataService) : ICommandHandler
{
    public string CommandType => "CheckGameAnalysisStatus";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var analysisInProcess = await queueDataService.RunQueryAsync(
            new GetGameAnalysisQuery()
            {
                OrderByCreatedAt = false,
                WithStatus = QueueStatus.Processing
            });

        foreach (var game in analysisInProcess)
        {
            var processingStatus = await GetProcessingStatus(game);
            if (processingStatus != KifubaraGameAnalysisState.Done)
                continue;
            
            await leagueDataService.ExecuteAsync(new UpdateMatchGameAnalysisCommand()
            {
                MatchId = game.MatchId,
                SetNewStatus = GameAnalysisStatus.Completed
            });

            await queueDataService.ExecuteAsync(new UpdateGameAnalysisCommand()
            {
                GameAnalysisId =  game.Id,
                NewStatus = QueueStatus.Completed,
                UpdateProcessedTime = true,
            });
        }
    }

    private async Task<string> GetProcessingStatus(GameAnalysis game)
    {
        return KifubaraGameAnalysisState.Done;
    }
}