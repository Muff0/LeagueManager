using Data;

namespace LeagueCoreService.ScheduledJobs;

public class PostDiscordPollScheduledJob : WeeklyScheduledJob
{
    public PostDiscordPollScheduledJob(QueueDataService queueDataService) : base(queueDataService)
    {
    }

    public override string Command => "PostDiscordPoll";
    protected override DayOfWeek Day { get; init; } = DayOfWeek.Monday;
    protected override TimeOnly Time { get; init; } = new TimeOnly(16,0);
}