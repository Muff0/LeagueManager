using Data;
using Data.Commands.Queue;
using Newtonsoft.Json;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class CleanupQueueScheduledJob(
    QueueDataService queueDataService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "CleanupQueue";
    
    
    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 100000
        });
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        await queueDataService.ExecuteAsync(new DeleteOldCommandMessagesCommand()
        {
            ExcludeCommandTypes = ["AwardApplicationCommand","RankChangeCommand"]
        });
    }
}