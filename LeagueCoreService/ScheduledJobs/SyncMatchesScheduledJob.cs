namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesScheduledJob : ScheduledJobBase
{
    public SyncMatchesScheduledJob() : base()
    {
    }

    public override string Command => "SyncMatches";
}