using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagoService;
using Newtonsoft.Json;
using OGS;
using Shared.Dto;
using Shared.Enum;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class QueueGameAnalysisScheduledJob(QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    OgsService ogsService,
    LeagoMainService leagoService,
    TimeIntervalSchedulerService schedulerService)
    : ScheduledJobBase<TimeIntervalSchedulerService>(schedulerService)
{
    public override string JobType => "QueueGameAnalysis";

    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        var season = await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery());
        var matchesToSchedule = await leagueDataService.RunQueryAsync(new GetMatchesQuery()
        {
            HasGameAnalysisUrl = false,
            HasScheduledGameAnalysis = false,
            IsPlayed = true,
            WithSeasonId = season!.Id,
            WithGameAnalysisStatus = GameAnalysisStatus.NotQueued
        });

        var queued = new List<MatchDto>();
        foreach (var match in matchesToSchedule)
        {
            var matchId = await ogsService.GetMatchIdFromLeagueId(match.OgsLeagueMatchId);
            string sgf = string.Empty;
            
            if (matchId == 0)
                // no OGS Link, we have to try getting the Leago sgf
                await leagoService.GetSgf(match.LeagoKey);
            else
                sgf = await ogsService.GetSgf(matchId);

            if (IsValidSgfFormat(sgf))
            {
                var gameAnalysis = new GameAnalysis()
                {
                    MatchId =  match.Id,
                    Sgf = sgf
                };

                await queueDataService.ExecuteAsync(new InsertGameAnalysisCommand()
                {
                    NewGameAnalysis = gameAnalysis
                });
                
                queued.Add(match.ToMatchDto());
            }
            else
            {
                await leagueDataService.ExecuteAsync(new UpdateMatchGameAnalysisCommand()
                {
                    MatchId = match.Id,
                    SetNewStatus = GameAnalysisStatus.NoRecord
                });
            }
        }

        await leagueDataService.ExecuteAsync(new SetMatchesGameAnalysisStatusCommand()
        {
            NewStatus = GameAnalysisStatus.Queued,
            Matches = queued.ToArray()
        });
    }
    
    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(
        new TimeIntervalJobSettings()
        {
            IntervalSeconds = 900
        });
    private bool IsValidSgfFormat(string sgf)
    {
        // very light check, the sources are trusted
        return !sgf.IsWhiteSpace() && sgf.Contains("RE[", StringComparison.InvariantCulture);
    }
}