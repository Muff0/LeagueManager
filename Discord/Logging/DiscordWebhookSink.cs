using System.Text;
using Newtonsoft.Json;
using Serilog.Core;
using Serilog.Events;

namespace Discord.Logging;
public sealed class DiscordWebhookSink : IBatchedLogEventSink, IDisposable
{
    private readonly string _webhookUrl;
    private readonly HttpClient _http = new();
    private readonly TimeSpan _repeatSuppressWindow;

    // Safe as a plain Dictionary — EmitBatchAsync calls are guaranteed non-concurrent.
    private readonly Dictionary<string, DateTime> _lastSentByKey = new();

    public DiscordWebhookSink(string webhookUrl, TimeSpan? repeatSuppressWindow = null)
    {
        _webhookUrl = webhookUrl;
        _repeatSuppressWindow = repeatSuppressWindow ?? TimeSpan.FromMinutes(5);
    }

    public async Task EmitBatchAsync(IReadOnlyCollection<LogEvent> batch)
    {
        var now = DateTime.UtcNow;

        // Collapse duplicate errors that landed in the same batch window
        foreach (var group in batch.GroupBy(BuildDedupeKey))
        {
            var events = group.ToList();
            var representative = events[0];

            if (_lastSentByKey.TryGetValue(group.Key, out var lastSent) &&
                now - lastSent < _repeatSuppressWindow)
                continue; // same error, still within suppression window — skip

            _lastSentByKey[group.Key] = now;
            await PostToDiscordAsync(representative, events.Count);
        }
    }

    public Task OnEmptyBatchAsync() => Task.CompletedTask;

    private static string BuildDedupeKey(LogEvent e)
    {
        var exceptionType = e.Exception?.GetType().FullName ?? "";
        var source = e.Properties.TryGetValue("SourceContext", out var sc) ? sc.ToString() : "";
        // MessageTemplate.Text, not RenderMessage() — "{SeasonId}" stays one
        // key regardless of which season ID triggered it
        return $"{source}|{e.MessageTemplate.Text}|{exceptionType}";
    }

    private async Task PostToDiscordAsync(LogEvent e, int countInBatch)
    {
        try
        {
            var embed = BuildEmbed(e, countInBatch);
            var body = JsonConvert.SerializeObject(new { embeds = new[] { embed } });
            await _http.PostAsync(_webhookUrl, new StringContent(body, Encoding.UTF8, "application/json"));
        }
        catch { /* a sink must never throw */ }
    }

    private static object BuildEmbed(LogEvent e, int countInBatch)
    {
        var color = e.Level switch
        {
            LogEventLevel.Warning => 0xFFA500,
            LogEventLevel.Error   => 0xE74C3C,
            LogEventLevel.Fatal   => 0x8B0000,
            _                     => 0x95A5A6
        };

        e.Properties.TryGetValue("SourceContext", out var sourceCtx);
        var fields = new List<object>();
        if (sourceCtx is not null)
            fields.Add(new { name = "Source", value = sourceCtx.ToString().Trim('"'), inline = true });
        if (e.Exception is not null)
            fields.Add(new { name = "Exception", value = $"```\n{Truncate(e.Exception.ToString(), 1000)}\n```" });

        var description = Truncate(e.RenderMessage(), 2000);
        if (countInBatch > 1)
            description += $"\n\n_(×{countInBatch} occurrences in this batch)_";

        return new
        {
            title = $"[{e.Level}] LeagueManager",
            description,
            color,
            fields,
            timestamp = e.Timestamp.UtcDateTime.ToString("o"),
            footer = new { text = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown env" }
        };
    }

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    public void Dispose() => _http.Dispose();
}