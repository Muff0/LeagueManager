using Microsoft.EntityFrameworkCore;

namespace Data.Commands.Queue;

public class DeleteOldCommandMessagesCommand : Command<QueueContext>
{
    public TimeSpan MaxAge = TimeSpan.FromDays(30);

    public string[] ExcludeCommandTypes { get; set; } = [];
    
    protected override void RunAction(QueueContext context)
    {
        var cutoffDate = DateTime.UtcNow.Subtract(MaxAge); 
        var query = context.CommandQueue.Where(cq => cq.ProcessedAtUtc < cutoffDate);
            if(ExcludeCommandTypes.Length>0)
                query = query.Where(cq => !ExcludeCommandTypes.Contains(cq.Type));
            context.CommandQueue.RemoveRange(query);
    }
}