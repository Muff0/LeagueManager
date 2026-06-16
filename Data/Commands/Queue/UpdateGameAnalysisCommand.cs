using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Commands.Queue;

public class UpdateGameAnalysisCommand : Command<QueueContext>
{
    public int GameAnalysisId { get; set; }
    public QueueStatus? NewStatus { get; set; }
    public string? StateUrl { get; set; } = null;

    public bool UpdateProcessedTime { get; set; } = false;

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);
        var existingGameAnalysis = context.GameAnalysis.FirstOrDefault(cm => cm.Id == GameAnalysisId);

        if (existingGameAnalysis == null)
            throw new InvalidOperationException("Invalid Queue Id");

        if(NewStatus != null)
            existingGameAnalysis.Status = NewStatus.Value;
        if(StateUrl != null)
            existingGameAnalysis.StateUrl = StateUrl;

        if (UpdateProcessedTime)
            existingGameAnalysis.ProcessedAtUtc = DateTime.UtcNow;

        context.Entry(existingGameAnalysis).State = EntityState.Modified;
    }
}