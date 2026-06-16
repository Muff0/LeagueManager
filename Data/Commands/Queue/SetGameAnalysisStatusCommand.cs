using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Commands.Queue;

public class SetGameAnalysisStatusCommand : Command<QueueContext>
{
    public int GameAnalysisId { get; set; }
    public QueueStatus NewStatus { get; set; }

    public bool UpdateProcessedTime { get; set; } = false;

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);
        var command = context.GameAnalysis.FirstOrDefault(cm => cm.Id == GameAnalysisId);

        if (command == null)
            throw new InvalidOperationException("Invalid Queue Id");

        command.Status = NewStatus;

        if (UpdateProcessedTime)
            command.ProcessedAtUtc = DateTime.UtcNow;

        context.Entry(command).State = EntityState.Modified;
    }
}