using Data.Model;
using LeagueCoreService.Services;

namespace LeagueCoreService.Queue;

public class SyncMatchesHandler(MainService mainService) 
    : ICommandHandler
{
    public string CommandType => "SyncMatches";
    
    public async Task HandleAsync(CommandMessage cmd)
        => await mainService.SyncMatches();
}