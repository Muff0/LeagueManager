using Data.Model;

namespace Data.Commands.Queue;

public class InsertCommandMessageCommand : Command<QueueContext>
{
    public CommandMessage? NewCommand { get; set; }

    protected override void RunAction(QueueContext context)
    {
        base.RunAction(context);

        if (NewCommand != null)
        {
            NewCommand.CreatedAtUtc = DateTime.UtcNow;
            context.Add(NewCommand);
        }
    }
}