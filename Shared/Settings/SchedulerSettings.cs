namespace Shared.Settings;

public class SchedulerSettings
{
    public DayOfWeek PollPostDay { get; set; } // e.g. "Monday"
    public int PollPostHour { get; set; } // e.g. 9 (UTC)
    public int SyncMatchesIntervalMinutes { get; set; } = 60;
    public int SendUpcomingMatchesIntervalMinutes { get; set; } = 15;
    public int UpcomingMatchesTimeSpanMinutes { get; set; } = 15;
    public int CleanupQueueIntervalDays { get; set; } = 1;
    public DayOfWeek UnconfirmedMatchReminderDay { get; set; }
    public int UnconfirmedMatchReminderHour { get; set; }
    public int QueueGameAnalysisIntervalMinutes { get; set; } = 15;
}