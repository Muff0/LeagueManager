using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class CheckGameAnalysisScheduledJob(QueueDataService queueDataService, CheckGameAnalysisSchedulerService schedulerService)
    : ScheduledJobBase<CheckGameAnalysisSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "CheckGameAnalysis";

}