using Data.Model;

namespace Data.Commands.Queue;

public class InsertJobRegistryCommand : Command<QueueContext>
{
    public JobRegistry? NewJobRegistry { get; set; }

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);

        if (NewJobRegistry != null)
        {
            context.Add(NewJobRegistry);
        }
    }
}