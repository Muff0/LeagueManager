namespace LeagueCoreService.ScheduledJobs;

public interface IScheduledJob
{
    string JobType { get; }
    string? DefaultSettingsJson => null;
    bool IsDue(DateTime now, DateTime? lastRunAt, string? settingsJson);
    Task ExecuteAsync(string? settingsJson, CancellationToken ct);
}