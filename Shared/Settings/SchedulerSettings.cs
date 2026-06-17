namespace Shared.Settings;

public class SchedulerSettings
{
    public int JobCacheReloadIntervalSeconds { get; set; } = 300;
    public int TickDelaySeconds { get; set; } = 30;
}