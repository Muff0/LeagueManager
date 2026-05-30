using Shared.Notifications;

public class NotificationService
{
    public event Action<NotificationMessage>? OnNotification;

    public void Notify(NotificationMessage message)
        => OnNotification?.Invoke(message);
}