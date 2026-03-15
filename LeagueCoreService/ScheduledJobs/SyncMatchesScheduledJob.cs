using Data;

namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesScheduledJob : ScheduledJobBase
{
    public SyncMatchesScheduledJob(QueueDataService queueDataService) : base(queueDataService)
    {
    }

    
    public override string Command => "SyncMatches";
}