using Data;
using Data.Model;
using LeagueCoreService.Services;
using Shared.Queue;

namespace LeagueCoreService.Queue;

public class SendNextPollNotificationHandler(MainService mainService) : ICommandHandler
{
    public string CommandType => "SendNextPollNotification";

    public async Task HandleAsync(CommandMessage cmd)
    {
        var payload = cmd.GetPayload<SendNextPollNotificationPayload>();

        if (payload == null)
            return;
        await mainService.SendNextPollNotification(payload.Time);
    }
    
}