using Microsoft.EntityFrameworkCore;

namespace Data.Commands.Queue;

public class DeleteOldCommandMessagesCommand : Command<QueueContext>
{
    public TimeSpan MaxAge = TimeSpan.FromDays(60);
    
    protected override void RunAction(QueueContext context)
    {
        var cutoffDate = DateTime.Now - MaxAge;
        context.CommandQueue.Where(cq => cq.ProcessedAtUtc < cutoffDate)
            .ExecuteDeleteAsync();
    }
}