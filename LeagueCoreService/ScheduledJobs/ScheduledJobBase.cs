using Data;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.ScheduledJobs;

public abstract class ScheduledJobBase : IScheduledJob
{
    protected QueueDataService _queueDataService;
    public ScheduledJobBase(QueueDataService queueDataService)
    {
        _queueDataService = queueDataService;
    }

    public DateTime LastRun { get; protected set; } = DateTime.MinValue;
    public abstract Task<bool> ShouldRun(DateTime now);

    public abstract string Command { get; }

    protected virtual string BuildPayload()
    {
        return "";
    }

    public virtual async Task Init()
    {
    }
    
    public async Task Enqueue()
    {
        var command = new CommandMessage()
        {
            CreatedAtUtc = DateTime.UtcNow,
            Type = Command,
            Payload = BuildPayload()
        };

        await _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = command,
        });

        LastRun = DateTime.Now;
        await OnEnqueued();
    }

    public async virtual Task OnEnqueued()
    {
        return;
    }
}