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
    LeagueDataService leagueDataService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "PostUpcomingMatches";

    
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        var settings = JsonConvert.DeserializeObject<PostUpcomingMatchesSettings>(settingsJson ?? string.Empty);
        if (settings  == null)
            throw new InvalidOperationException($"Invalid settings: {nameof(PostUpcomingMatchesScheduledJob)}, Json: {settingsJson}");
        
        var resm = await GetUpcomingMatches(settings.UpcomingMatchesTimeSpanSeconds);

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

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 300
        });
    private async Task<Match[]> GetUpcomingMatches(int secondsInterval = 300)
    {
        var query = new GetMatchesByTimeQuery
        {
            InlcudePlayers = true,
            IncludeCompleted = false,
            IncludeNotConfirmed = false,
            IsNotificationSent = false,
            TimeFromUtc = DateTime.Now.ToUniversalTime(),
            TimeToUtc = DateTime.Now.AddSeconds(secondsInterval).ToUniversalTime(),
            Count = 5
        };

        var res = await leagueDataService.RunQueryAsync(query);

        return res.ToArray();
    }

}