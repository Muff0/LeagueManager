namespace Shared.Settings;

public class DayOfWeekJobSettings : IDayOfWeekJobSettings
{
    public DayOfWeek DayOfWeek { get; set; }
    public int HourOfDayUtc { get; set; }
}