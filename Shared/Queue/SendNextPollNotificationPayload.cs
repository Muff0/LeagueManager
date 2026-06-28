using Shared.Enum;

namespace Shared.Queue;

public class SendNextPollNotificationPayload : CommandPayload
{
    public string Message { get; set; } = string.Empty;
    public DiscordNotificationType DiscordNotificationType { get; set; } = DiscordNotificationType.Admin;
}