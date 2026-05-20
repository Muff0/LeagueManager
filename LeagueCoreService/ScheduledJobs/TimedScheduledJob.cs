using Data;

namespace LeagueCoreService.ScheduledJobs;

public abstract class TimedScheduledJob(QueueDataService queueDataService) : ScheduledJobBase(queueDataService)
{
    
    public virtual TimeSpan Interval { get; set; } = TimeSpan.FromHours(1);

    
    public override async Task<bool> ShouldRun(DateTime now)
    {
        return now > LastRun.Add(Interval);
    }
}