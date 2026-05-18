using Data;

namespace LeagueCoreService.ScheduledJobs;

public class CleanupQueueScheduledJob : ScheduledJobBase
{
    public CleanupQueueScheduledJob(QueueDataService queueDataService) : base(queueDataService)
    {
    }

    
    public override string Command => "CleanupQueue";
}