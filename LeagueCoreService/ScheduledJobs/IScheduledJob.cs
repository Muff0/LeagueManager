namespace LeagueCoreService.ScheduledJobs;

public interface IScheduledJob
{
    string JobType { get; }
    string? DefaultSettingsJson => null;
    bool IsDue(DateTime nowUtc, DateTime? lastRunAtUtc, string? settingsJson);
    Task ExecuteAsync(string? settingsJson, CancellationToken ct);
}