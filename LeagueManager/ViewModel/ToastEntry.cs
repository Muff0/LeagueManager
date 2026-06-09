using Shared.Notifications;

namespace LeagueManager.ViewModel;

internal class ToastEntry(NotificationMessage message)
{
    public Guid Id { get; } = Guid.NewGuid();
    public NotificationMessage Message { get; } = message;
    public bool Exiting { get; set; }
}