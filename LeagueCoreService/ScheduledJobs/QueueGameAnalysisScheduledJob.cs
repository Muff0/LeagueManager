using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class QueueGameAnalysisScheduledJob(QueueDataService queueDataService, SyncMatchesSchedulerService schedulerService)
    : ScheduledJobBase<SyncMatchesSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "QueueGameAnalysis";
}