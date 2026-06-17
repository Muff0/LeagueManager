namespace Shared.Settings;

public interface IDayOfWeekJobSettings
{
     DayOfWeek DayOfWeek { get;}
    int HourOfDayUtc { get; }
}