using Data;
using Data.Commands.Match;
using Data.Model;
using Data.Queries;
using Discord;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Dto.Discord;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class PostUpcomingMatchesScheduledJob(
    DiscordService discordService,
    TimeIntervalSchedulerService schedulerService,
    LeagueDataService leagueDataService,
    IOptions<SchedulerSettings> settings)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "PostUpcomingMatches";

    
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        await SendUpcomingMatchesNotification();
    }

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 300
        });
    public async Task<Match[]> GetUpcomingMatches()
    {
        var query = new GetMatchesByTimeQuery
        {
            InlcudePlayers = true,
            IncludeCompleted = false,
            IncludeNotConfirmed = false,
            IsNotificationSent = false,
            TimeFromUTC = DateTime.Now.ToUniversalTime(),
            TimeToUTC = DateTime.Now.AddMinutes(settings.Value.UpcomingMatchesTimeSpanMinutes).ToUniversalTime(),
            Count = 5
        };

        var res = await leagueDataService.RunQueryAsync(query);

        return res.ToArray();
    }

    public async Task SendUpcomingMatchesNotification()
    {
            var resm = await GetUpcomingMatches();

            if (resm.Length == 0)
                return;
            var dto = resm.Select(mm => mm.ToMatchDto()).ToArray();
            await discordService.SendUpcomingMatchesNotification(new SendUpcomingMatchesNotificationInDto
            {
                Matches = dto
            });
            await leagueDataService.ExecuteAsync(new SetMatchesNotifiedCommand
            {
                Matches = dto
            });
    }
}