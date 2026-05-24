// Discord/Logging/DiscordWebhookSink.cs

using System.Text;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;

namespace  Discord.Logging
{
    
public sealed class DiscordWebhookSink : ILogEventSink, IDisposable
{
    private readonly string _webhookUrl;
    private readonly HttpClient _http = new();
    private readonly LogEventLevel _minimumLevel;
    // Basic rate-limit guard: Discord webhooks allow ~5 req/2s per URL
    private readonly SemaphoreSlim _throttle = new(1, 1);

    public DiscordWebhookSink(string webhookUrl, LogEventLevel minimumLevel = LogEventLevel.Error)
    {
        _webhookUrl = webhookUrl;
        _minimumLevel = minimumLevel;
    }

    public void Emit(LogEvent logEvent)
    {
        if (logEvent.Level < _minimumLevel) return;
        // Fire-and-forget — never block the caller
        _ = EmitAsync(logEvent);
    }

    private async Task EmitAsync(LogEvent logEvent)
    {
        await _throttle.WaitAsync();
        try
        {
            var embed = BuildEmbed(logEvent);
            var body = JsonConvert.SerializeObject(new { embeds = new[] { embed } });
            await _http.PostAsync(_webhookUrl,
                new StringContent(body, Encoding.UTF8, "application/json"));
            await Task.Delay(500); // stay well under rate limit
        }
        catch { /* logging must never throw */ }
        finally { _throttle.Release(); }
    }

    private static object BuildEmbed(LogEvent e)
    {
        var color = e.Level switch
        {
            LogEventLevel.Warning  => 0xFFA500,
            LogEventLevel.Error    => 0xE74C3C,
            LogEventLevel.Fatal    => 0x8B0000,
            _                      => 0x95A5A6
        };

        // Source context = the class name that called ILogger<T>
        e.Properties.TryGetValue("SourceContext", out var sourceCtx);

        var fields = new List<object>();

        if (sourceCtx is not null)
            fields.Add(new { name = "Source", value = sourceCtx.ToString().Trim('"'), inline = true });

        if (e.Exception is not null)
            fields.Add(new
            {
                name  = "Exception",
                value = $"```\n{Truncate(e.Exception.ToString(), 1000)}\n```",
                inline = false
            });

        return new
        {
            title       = $"[{e.Level}] LeagueManager",
            description = Truncate(e.RenderMessage(), 2000),
            color,
            fields,
            timestamp   = e.Timestamp.UtcDateTime.ToString("o"),
            footer      = new { text = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown env" }
        };
    }

    private static string Truncate(string s, int max)
        => s.Length <= max ? s : s[..max] + "…";

    public void Dispose() => _http.Dispose();
}
}