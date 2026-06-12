using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries;

public class GetMatchesQuery : Query<LeagueContext, Match>
{
    public bool IncludePlayers { get; set; }
    public override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        
        
        // Returning Everything for now
        var query =  base.BuildQuery(context);

        if (IncludePlayers)
            query = query.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player);

        return query;
    }
}