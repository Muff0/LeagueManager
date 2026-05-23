namespace Shared.Settings;

public class PollSchedulerSettings
{
    public DayOfWeek PollPostDay { get; set; }   // e.g. "Monday"
    public int PollPostHour { get; set; }          // e.g. 9 (UTC)
}