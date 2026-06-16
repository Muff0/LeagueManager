using Data;
using Data.Queries;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class QueueGameAnalysisScheduledJob(QueueDataService queueDataService, QueueGameAnalysisSchedulerService schedulerService)
    : ScheduledJobBase<QueueGameAnalysisSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "QueueGameAnalysis";

}