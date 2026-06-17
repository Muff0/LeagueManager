using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Kifubara.Models;
using Newtonsoft.Json;
using Shared.Enum;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class CheckGameAnalysisScheduledJob(QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 600
        });
    public override string JobType => "CheckGameAnalysis";

    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
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