using Data;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.ScheduledJobs;

public abstract class ScheduledJobBase : IScheduledJob
{
    public ScheduledJobBase()
    {
    }
    
    public DateTime LastRun { get; protected set; }
    public TimeSpan Interval { get; } = TimeSpan.FromHours(1);

    public bool ShouldRun(DateTime now)
    {
        return now > LastRun.Add(Interval);
    }

    public abstract string Command { get; }

    protected virtual string BuildPayload()
    {
        return "";
    }
    
    public async Task Enqueue(QueueDataService queueDataService)
    {
        var command = new CommandMessage()
        {
            CreatedAtUtc = DateTime.UtcNow,
            Type = Command,
            Payload = BuildPayload()
        };

        await queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = command,
        });

        LastRun = DateTime.Now;
    }
}