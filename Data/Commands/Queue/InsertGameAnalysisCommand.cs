using Data.Model;

namespace Data.Commands.Queue;

public class InsertGameAnalysisCommand : Command<QueueContext>
{
    public GameAnalysis? NewGameAnalysis { get; set; }

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);

        if (NewGameAnalysis != null)
        {
            NewGameAnalysis.CreatedAtUtc = DateTime.UtcNow;
            context.Add(NewGameAnalysis);
        }
    }
}