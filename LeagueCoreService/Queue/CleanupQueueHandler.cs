using Data;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.Queue;

public class CleanupQueueHandler(QueueDataService queueDataService)
    : ICommandHandler
{
    public string CommandType => "CleanupQueue";

    public async Task HandleAsync(CommandMessage cmd)
    {
        await queueDataService.ExecuteAsync(new DeleteOldCommandMessagesCommand()
        {
            ExcludeCommandTypes = ["AwardApplicationCommand","RankChangeCommand"]
        });
    }
}