using Data;
using Data.Queries;
using Microsoft.Extensions.Options;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class PostUpcomingMatchesScheduledJob(
    QueueDataService queueDataService,
    PostUpcomingMatchesSchedulerService schedulerService,
    LeagueDataService leagueDataService,
    IOptions<SchedulerSettings> settings)
    : ScheduledJobBase<PostUpcomingMatchesSchedulerService>(queueDataService, schedulerService)
{
    public override string Command { get; } = "PostUpcomingMatches";

    public override async Task<bool> ShouldRun(DateTime now)
    {
        var shouldRun = await base.ShouldRun(now);
        if (!shouldRun)
            return false;

        var isUpcoming = await leagueDataService.CountAsync(new GetMatchesByTimeQuery
        {
            InlcudePlayers = true,
            IncludeCompleted = false,
            IncludeNotConfirmed = false,
            IsNotificationSent = false,
            TimeFromUTC = DateTime.Now.ToUniversalTime(),
            TimeToUTC = DateTime.Now.ToUniversalTime().AddMinutes(settings.Value.UpcomingMatchesTimeSpanMinutes),
            Count = 5
        });

        return isUpcoming > 0;
    }
}