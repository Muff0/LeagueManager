using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Queries;
using Kifubara;
using Newtonsoft.Json;
using Shared.Enum;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class StartGameAnalysisScheduledJob(QueueDataService queueDataService, 
    KifubaraService kifubaraService,
    LeagueDataService leagueDataService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "StartGameAnalysis";
    
    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 900
        });
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
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