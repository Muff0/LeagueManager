using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Newtonsoft.Json;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public abstract class ScheduledJobBase<T>(T schedulerService)
    : IScheduledJob where T : IJobSchedulerService
{
    public abstract string JobType { get; }
    public virtual bool IsDue(DateTime now, DateTime? lastRunAt, string? settingsJson)
    {
        if (!lastRunAt.HasValue)
            return true;
        var nextOccurrence = schedulerService.GetNextOccurrence(lastRunAt.Value,settingsJson, now);
        return (now >= nextOccurrence);
    }

    public virtual string? DefaultSettingsJson => "";

    public abstract Task ExecuteAsync(string? settingsJson, CancellationToken ct);
}