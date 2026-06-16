using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class StartGameAnalysisScheduledJob(QueueDataService queueDataService, StartGameAnalysisSchedulerService schedulerService)
    : ScheduledJobBase<StartGameAnalysisSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "StartGameAnalysis";

}