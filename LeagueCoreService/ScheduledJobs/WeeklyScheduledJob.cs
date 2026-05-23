using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public abstract class WeeklyScheduledJob<T> : ScheduledJobBase where T : IJobSchedulerService
{
    protected T _schedulerService;

    protected WeeklyScheduledJob(QueueDataService queueDataService, T schedulerService)
        : base(queueDataService)
    {
        _schedulerService = schedulerService;
    }

    public override Task<bool> ShouldRun(DateTime now)
    {   
        var nextOccurrence = _schedulerService.GetNextOccurrence(LastRun, now);
        return Task.FromResult(now >= nextOccurrence);
    }
}