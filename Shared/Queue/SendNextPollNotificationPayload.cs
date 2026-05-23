using Shared.Enum;

namespace Shared.Queue;

public class SendNextPollNotificationPayload : ICommandPayload
{
    public string Message { get; set; } = string.Empty;
    public DiscordNotificationType DiscordNotificationType { get; set; } = DiscordNotificationType.Admin;
}