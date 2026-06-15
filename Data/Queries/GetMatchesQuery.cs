using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Queries;

public class GetMatchesQuery : Query<LeagueContext, Match>
{
    public bool IncludePlayers { get; set; }
    public bool? HasGameAnalysisUrl { get; set; } = null;
    public bool? HasScheduledGameAnalysis { get; set; } = null;
    public bool? IsPlayed { get; set; } = null;
    public int? SeasonId { get; set; } = null;
    public int? Round { get; set; } = null;
    
    public override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        
        // Returning Everything for now
        var query =  base.BuildQuery(context);

        if (IncludePlayers)
            query = query.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player);
        if (SeasonId.HasValue)
            query = query.Where(mm => mm.SeasonId == SeasonId);
        if (Round.HasValue)
            query = query.Where(mm => mm.Round == Round);
        if (HasGameAnalysisUrl.HasValue)
            query = query.Where(mm => (mm.GameAnalysisUrl != null) == HasGameAnalysisUrl.Value);
        if (HasScheduledGameAnalysis.HasValue)
            query = query.Where(mm => (mm.GameAnalysisStatus != GameAnalysisStatus.NotQueued)  == HasScheduledGameAnalysis.Value);
        if (IsPlayed.HasValue)
            query = query.Where(mm => mm.PlayerMatches.Any(pm => pm.Outcome == PlayerMatchOutcome.Loss) == IsPlayed.Value);
        return query;
    }
}