using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public abstract class ScheduledJobBase<T>(QueueDataService queueDataService, T schedulerService)
    : IScheduledJob where T : IJobSchedulerService
{
    public DateTime LastRun { get; protected set; } = DateTime.MinValue;

    public abstract string Command { get; }

    public virtual Task<bool> ShouldRun(DateTime now)
    {
        var nextOccurrence = schedulerService.GetNextOccurrence(LastRun, now);
        return Task.FromResult(now >= nextOccurrence);
    }


    public virtual async Task Init()
    {
        var lastRun = await queueDataService.TakeFirstAsync(new GetCommandMessageLastRunQuery
        {
            CommandMessageType = Command
        });
        if (lastRun != null)
            LastRun = lastRun.ProcessedAtUtc;
    }


    public async Task Enqueue()
    {
        var command = new CommandMessage
        {
            CreatedAtUtc = DateTime.UtcNow,
            Type = Command,
            Payload = BuildPayload()
        };

        await queueDataService.ExecuteAsync(new InsertCommandMessageCommand
        {
            NewCommand = command
        });

        LastRun = DateTime.Now;
        await OnEnqueued();
    }

    protected virtual string BuildPayload()
    {
        return "";
    }

    public virtual async Task OnEnqueued()
    {
    }
}