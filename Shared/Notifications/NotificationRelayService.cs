using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.Notifications;

public class NotificationRelayService(
    NotificationDispatcher dispatcher,
    IHubContext<NotificationHub> hubContext,
    ILogger<NotificationRelayService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var message in dispatcher.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await hubContext.Clients.All.SendAsync("Notification", message, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to broadcast notification");
            }
        }
    }
}