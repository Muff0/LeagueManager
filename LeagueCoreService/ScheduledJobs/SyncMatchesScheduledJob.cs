namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesScheduledJob : ScheduledJobBase
{
    public SyncMatchesScheduledJob() : base()
    {
        Command = "SyncMatches";
    }
    
}