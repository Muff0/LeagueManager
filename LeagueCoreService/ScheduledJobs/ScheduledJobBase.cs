using Data;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.ScheduledJobs;

public abstract class ScheduledJobBase : IScheduledJob
{
    private QueueDataService _queueDataService;
    public ScheduledJobBase(QueueDataService queueDataService)
    {
        _queueDataService = queueDataService;
    }
    
    public DateTime LastRun { get; protected set; }
    public virtual TimeSpan Interval { get; } = TimeSpan.FromHours(1);

    public virtual async Task<bool> ShouldRun(DateTime now)
    {
        return now > LastRun.Add(Interval);
    }

    public abstract string Command { get; }

    protected virtual string BuildPayload()
    {
        return "";
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
    }
}