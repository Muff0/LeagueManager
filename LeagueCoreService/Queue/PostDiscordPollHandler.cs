using Data.Model;
using LeagueCoreService.Services;

namespace LeagueCoreService.Queue;

public class PostDiscordPollHandler(MainService mainService) 
    : ICommandHandler
{
    
    public string CommandType => "PostDiscordPoll";
    
    public async Task HandleAsync(CommandMessage cmd)
        => await mainService.PostNextDiscordPoll();
}