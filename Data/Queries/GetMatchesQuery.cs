using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Queries;

public class GetMatchesQuery : Query<LeagueContext, Match>
{
    public bool IncludePlayers { get; set; }
    public bool IncludeSeason { get; set; }
    public bool? HasGameAnalysisUrl { get; set; }
    public bool? HasScheduledGameAnalysis { get; set; }
    public bool? IsPlayed { get; set; }
    public int? WithId { get; set; }
    public int? WithSeasonId { get; set; }
    public int? WithRound { get; set; }
    public GameAnalysisStatus? WithGameAnalysisStatus { get; set; } = null;
    
    public override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        var query =  base.BuildQuery(context);

        if (IncludeSeason)
            query = query.Include(mm => mm.Season);
        if (IncludePlayers)
            query = query.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player);
        if (WithId.HasValue)
            query = query.Where(mm => mm.Id == WithId.Value);
        if (WithSeasonId.HasValue)
            query = query.Where(mm => mm.SeasonId == WithSeasonId);
        if (WithRound.HasValue)
            query = query.Where(mm => mm.Round == WithRound);
        if (HasGameAnalysisUrl.HasValue)
            query = query.Where(mm => (mm.GameAnalysisUrl != null) == HasGameAnalysisUrl.Value);
        if (HasScheduledGameAnalysis.HasValue)
            query = query.Where(mm => (mm.GameAnalysisStatus != GameAnalysisStatus.NotQueued)  == HasScheduledGameAnalysis.Value);
        if (IsPlayed.HasValue)
            query = query.Where(mm => mm.PlayerMatches.Any(pm => pm.Outcome == PlayerMatchOutcome.Loss) == IsPlayed.Value);
        if (WithGameAnalysisStatus.HasValue)
            query = query.Where(mm => mm.GameAnalysisStatus == WithGameAnalysisStatus.Value);
        
        return query;
    }
}