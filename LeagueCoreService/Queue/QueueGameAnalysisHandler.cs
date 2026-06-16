using Data;
using Data.Commands.Match;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagoService;
using OGS;
using OGS.Model;
using Shared.Dto;
using Shared.Enum;

namespace LeagueCoreService.Queue;

public class QueueGameAnalysisHandler(LeagueDataService leagueDataService,
    QueueDataService queueDataService,
    OGSService ogsService,
    LeagoMainService leagoService)
    : ICommandHandler
{
    public string CommandType => "QueueGameAnalysis";

    public async Task HandleAsync(CommandMessage cmd)
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

        leagueDataService.ExecuteAsync(new SetMatchesGameAnalysisStatusCommand()
        {
            NewStatus = GameAnalysisStatus.Queued,
            Matches = queued.ToArray()
        });
    }
    
    private bool IsValidSgfFormat(string sgf)
    {
        // very light check, the sources are trusted
        return !sgf.IsWhiteSpace() && sgf.Contains("RE[", StringComparison.InvariantCulture);
    }
}