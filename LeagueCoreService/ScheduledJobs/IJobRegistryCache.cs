using Data.Model;

namespace LeagueCoreService.ScheduledJobs;

public interface IJobRegistryCache
{
    bool IsEnabled(string jobType);
    string? GetSettingsJson(string jobType);
    DateTime? GetLastRunAt(string jobType);
    void SetLastRunAt(string jobType, DateTime lastRunAt);
    void Reload(IReadOnlyList<JobRegistry> entries);
    void UpdateSettings(string jobType, bool isEnabled, string? settingsJson);
    IReadOnlySet<string> RegisteredTypes { get; }
}
