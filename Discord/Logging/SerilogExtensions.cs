using Serilog;
using Serilog.Configuration;
using Serilog.Events;

namespace Discord.Logging;
// Discord/Logging/SerilogExtensions.cs
public static class SerilogExtensions
{
    public static LoggerConfiguration WriteToDiscordWebhook(
        this LoggerSinkConfiguration sinks,
        string webhookUrl,
        LogEventLevel minimumLevel = LogEventLevel.Error)
    {
        return sinks.Sink(new DiscordWebhookSink(webhookUrl, minimumLevel), minimumLevel);
    }
}