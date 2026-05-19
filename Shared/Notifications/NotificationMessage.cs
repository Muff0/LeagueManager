namespace Shared.Notifications;
public record NotificationMessage(
    string Title,
    string? Detail,
    NotificationLevel Level,
    string Source,
    DateTimeOffset Timestamp)
{
    public static NotificationMessage Success(string title, string? detail, string source)
        => new(title, detail, NotificationLevel.Success, source, DateTimeOffset.UtcNow);

    public static NotificationMessage Error(string title, string? detail, string source)
        => new(title, detail, NotificationLevel.Error, source, DateTimeOffset.UtcNow);

    public static NotificationMessage Info(string title, string? detail, string source)
        => new(title, detail, NotificationLevel.Info, source, DateTimeOffset.UtcNow);
}

public enum NotificationLevel { Info, Success, Warning, Error }