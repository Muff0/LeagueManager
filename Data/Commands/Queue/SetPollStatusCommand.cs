using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Commands.Queue;

public class SetPollStatusCommand : Command<QueueContext>
{
    public int PollId { get; set; }
    public QueueStatus NewStatus { get; set; }

    public bool UpdateProcessedTime { get; set; } = true;

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);
        var poll = context.PollQueue.FirstOrDefault(cm => cm.Id == PollId);

        if (poll == null)
            throw new InvalidOperationException("Invalid Queue Id");

        poll.Status = NewStatus;

        if (UpdateProcessedTime)
            poll.ProcessedAtUtc = DateTime.UtcNow;

        context.Entry(poll).State = EntityState.Modified;
    }
}