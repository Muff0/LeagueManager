using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class SendGameAnalysisScheduledJob(QueueDataService queueDataService, SendGameAnalysisSchedulerService schedulerService)
    : ScheduledJobBase<SendGameAnalysisSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "SendGameAnalysis";

}