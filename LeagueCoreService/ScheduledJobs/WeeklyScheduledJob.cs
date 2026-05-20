using Data;

namespace LeagueCoreService.ScheduledJobs;

public abstract class WeeklyScheduledJob : ScheduledJobBase
{
    protected abstract DayOfWeek Day { get; init; }
    protected abstract TimeOnly Time { get; init; } // UTC

    protected WeeklyScheduledJob(QueueDataService queueDataService)
        : base(queueDataService) { }

    public override Task<bool> ShouldRun(DateTime now)
    {   
        var lastOccurrence = GetLastOccurrence(now);
        return Task.FromResult(LastRun < lastOccurrence);
    }

    private DateTime GetLastOccurrence(DateTime now)
    {
        int daysBack = ((int)now.DayOfWeek - (int)Day + 7) % 7;
        var candidate = now.Date.AddDays(-daysBack) + Time.ToTimeSpan();

        if (candidate > now)
            candidate = candidate.AddDays(-7);

        return candidate;
    }
}