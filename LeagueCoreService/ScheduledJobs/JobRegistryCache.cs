using Data.Migrations.Queue;

namespace LeagueCoreService.ScheduledJobs;

public class JobRegistryCache : IJobRegistryCache
{
    private record Snapshot(bool IsEnabled, string? SettingsJson, DateTime? LastRunAt);

    private volatile IReadOnlyDictionary<string, Snapshot> _cache
        = new Dictionary<string, Snapshot>();

    public IReadOnlySet<string> RegisteredTypes { get; private set; }
        = new HashSet<string>();

    public bool IsEnabled(string jobType)
        => !_cache.TryGetValue(jobType, out var s) || s.IsEnabled;

    public string? GetSettingsJson(string jobType)
        => _cache.TryGetValue(jobType, out var s) ? s.SettingsJson : null;

    public DateTime? GetLastRunAt(string jobType)
        => _cache.TryGetValue(jobType, out var s) ? s.LastRunAt : null;

    public void SetLastRunAt(string jobType, DateTime lastRunAt)
    {
        if (!_cache.TryGetValue(jobType, out var s)) return;
        _cache = new Dictionary<string, Snapshot>(_cache)
        {
            [jobType] = s with { LastRunAt = lastRunAt }
        };
    }

    public void UpdateSettings(string jobType, bool isEnabled, string? settingsJson)
    {
        if (!_cache.TryGetValue(jobType, out var s)) return;
        _cache = new Dictionary<string, Snapshot>(_cache)
        {
            [jobType] = s with { IsEnabled = isEnabled, SettingsJson = settingsJson }
        };
    }

    public void Reload(IReadOnlyList<Data.Model.JobRegistry> entries, IReadOnlySet<string> registeredTypes)
    {
        _cache = entries.ToDictionary(
            e => e.JobType,
            e => new Snapshot(e.IsEnabled, e.SettingsJson, e.LastRunAtUtc)); 
        RegisteredTypes = registeredTypes;
    }

}