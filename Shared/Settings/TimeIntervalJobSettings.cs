namespace Shared.Settings;

public class TimeIntervalJobSettings : ITimeIntervalJobSettings
{
    public int IntervalSeconds { get; set; } = 60;
}