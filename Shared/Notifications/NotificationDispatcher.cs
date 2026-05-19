using System.Threading.Channels;

namespace Shared.Notifications;
public interface INotificationDispatcher
{
    void Dispatch(NotificationMessage message);
}

// Singleton — wraps a Channel<T>
public class NotificationDispatcher : INotificationDispatcher
{
    private readonly Channel<NotificationMessage> _channel =
        Channel.CreateUnbounded<NotificationMessage>(
            new UnboundedChannelOptions { SingleReader = true });

    public void Dispatch(NotificationMessage message)
        => _channel.Writer.TryWrite(message);  // fire-and-forget, never blocks

    public ChannelReader<NotificationMessage> Reader => _channel.Reader;
}