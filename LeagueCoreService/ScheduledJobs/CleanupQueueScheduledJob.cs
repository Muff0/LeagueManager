using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class CleanupQueueScheduledJob(
    QueueDataService queueDataService,
    CleanupQueueSchedulerService schedulerService,
    LeagueDataService leagueDataService)
    : ScheduledJobBase<CleanupQueueSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "CleanupQueue";
}