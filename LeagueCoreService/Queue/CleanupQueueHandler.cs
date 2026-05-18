using Data.Model;
using LeagueCoreService.Services;

namespace LeagueCoreService.Queue;

public class CleanupQueueHandler(MainService mainService) 
    : ICommandHandler
{
    public string CommandType => "CleanupQueue";
    
    public async Task HandleAsync(CommandMessage cmd)
        => await mainService.CleanupQueue();
}