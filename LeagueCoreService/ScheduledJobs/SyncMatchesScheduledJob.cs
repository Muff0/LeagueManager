using Data;

namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesScheduledJob : TimedScheduledJob
{
    public SyncMatchesScheduledJob(QueueDataService queueDataService) : base(queueDataService)
    {
    }

    
    public override string Command => "SyncMatches";
}