using Shared.Enum;

namespace Shared.Queue;

public class SendNextPollNotificationPayload : ICommandPayload
{
    public DateTime Time { get; set; } 
}