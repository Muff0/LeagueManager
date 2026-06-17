using Microsoft.EntityFrameworkCore;

namespace Data.Commands.Queue;

public class UpdateJobLastRunCommand : Command<QueueContext>
{
    public string JobType { get; set; } = string.Empty;
    public DateTime LastRun { get; set; }
    protected override void RunAction(QueueContext context)
    {
        var existing = context.JobRegistry.FirstOrDefault(j => j.JobType == JobType);
        if (existing is null)
            throw new InvalidOperationException($"Job type {JobType} does not exist") ;
        
        existing.LastRunAtUtc = LastRun;
    }
}