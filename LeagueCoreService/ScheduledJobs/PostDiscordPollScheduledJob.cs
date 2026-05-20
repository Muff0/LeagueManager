using Data;
using Data.Queries;

namespace LeagueCoreService.ScheduledJobs;

public class PostDiscordPollScheduledJob : WeeklyScheduledJob
{
    public PostDiscordPollScheduledJob(QueueDataService queueDataService) : base(queueDataService)
    {
    }

    public override string Command => "PostDiscordPoll";
    protected override DayOfWeek Day { get; init; } = DayOfWeek.Monday;
    protected override TimeOnly Time { get; init; } = new TimeOnly(16,0);

    public override async Task Init()
    {
        var lastRun = await _queueDataService.RunQueryAsync(new GetLastPostedPollQuery());
        if (lastRun != null)
            LastRun = lastRun.ProcessedAtUtc.GetValueOrDefault();
    }
}