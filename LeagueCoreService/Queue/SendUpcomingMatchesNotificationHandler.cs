using Data.Model;
using LeagueCoreService.Services;

namespace LeagueCoreService.Queue;

public class SendUpcomingMatchesNotificationHandler(MainService mainService) 
    : ICommandHandler
{
    public string CommandType => "SendUpcomingMatchesNotification";
    
    public async Task HandleAsync(CommandMessage cmd)
        => await mainService.SendUpcomingMatchesNotification();
}