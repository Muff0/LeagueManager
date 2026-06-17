using Data;
using Data.Commands.Match;
using Data.Model;
using Data.Queries;
using LeagoService;
using LeagueCoreService.ScheduledJobs;
using Newtonsoft.Json;
using Shared.Dto;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class SyncMatchesSchedulerJobs(
    LeagueDataService leagueDataService,
    LeagoMainService leagoService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "SyncMatches";

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 60
        });
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        await SyncMatches();
    }
    
    public async Task SyncMatchesForRound(int round)
    {
        var activeSeason = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        if (activeSeason == null)
            return;

        var getMatchesRes = await leagoService.GetMatches(new GetMatchesInDto
        {
            RoundKey = round,
            TournamentKey = activeSeason.LeagoL2Key,
            MatchesCount = 100
        });

        if (getMatchesRes != null)
            await leagueDataService.ExecuteAsync(
                new AddOrUpdateMatchesCommand
                {
                    ToUpdate = getMatchesRes.Matches,
                    SeasonId = activeSeason.Id,
                    Round = round
                });
    }

    private async Task SyncMatches()
    {
        for (var ii = 0; ii < 5; ii++)
            await SyncMatchesForRound(ii + 1);
    }
}