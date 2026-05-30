using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Queries;

public class GetUnconfirmedMatchesQuery: Query<LeagueContext, Match>
{
    public bool IncludePlayers { get; set; }
    public int SeasonId { get; set; }

    public override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);

        if (IncludePlayers)
            query = query.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player);

        query = query.Where(m => m.SeasonId == SeasonId
                && !m.IsComplete
                && m.PlayerMatches.All(pm => pm.Outcome == PlayerMatchOutcome.NotReported)
                && m.PlayerMatches.Any(pm => !pm.HasConfirmed));
        
        return query;
    }
}
