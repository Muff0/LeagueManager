using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Shared.Notifications;


namespace LeagueManager.Services
{
    public class NotificationService : IAsyncDisposable
    {
        private readonly HubConnection _hub;

        public event Action<NotificationMessage>? OnNotification;

        public NotificationService(NavigationManager nav)
        {
            _hub = new HubConnectionBuilder()
                .WithUrl(nav.ToAbsoluteUri("/hubs/notifications"))
                .WithAutomaticReconnect()
                .Build();

            _hub.On<NotificationMessage>("Notification", msg => OnNotification?.Invoke(msg));
        }

        public async Task StartAsync() => await _hub.StartAsync();

        public async ValueTask DisposeAsync() => await _hub.DisposeAsync();
    }
}