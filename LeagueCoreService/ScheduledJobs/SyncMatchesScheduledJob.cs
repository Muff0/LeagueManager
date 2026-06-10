using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesScheduledJob(QueueDataService queueDataService, SyncMatchesSchedulerService schedulerService)
    : ScheduledJobBase<SyncMatchesSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "SyncMatches";
}