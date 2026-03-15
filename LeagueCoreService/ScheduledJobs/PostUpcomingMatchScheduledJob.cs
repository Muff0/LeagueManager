using Data;
using Data.Queries;

namespace LeagueCoreService.ScheduledJobs;

public class PostUpcomingMatchScheduledJob : ScheduledJobBase
{
    private LeagueDataService _leagueDataService;
    public PostUpcomingMatchScheduledJob(QueueDataService queueDataService,
        LeagueDataService leagueDataService) : base(queueDataService)
    {
        _leagueDataService = leagueDataService;
    }
    public override string Command { get; } = "PostUpcomingMatch";

    public override TimeSpan Interval => TimeSpan.FromMinutes(1);

    public override async Task<bool> ShouldRun(DateTime now)
    {
        var shouldRun = await base.ShouldRun(now);
        if (!shouldRun)
            return false;

        var isUpcoming = await _leagueDataService.RunQueryAsync(
            new UpcomingMatchesQuery());

        return true;
    }
}