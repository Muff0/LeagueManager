using Data;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.ScheduledJobs;

public class ScheduledJobBase : IScheduledJob
{
    public ScheduledJobBase()
    {
    }
    
    protected DateTime lastRun;
    public TimeSpan Interval { get; } = TimeSpan.FromHours(1);

    public bool ShouldRun(DateTime now)
    {
        return now > lastRun.Add(Interval);
    }

    public string Command { get; set; }

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
    }
}