using Data.Model;
using Shared.Infrastructure;

namespace Data.Queries;

public class UpcomingMatchesQuery : Scalar<LeagueContext,Match>
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(5);
    protected override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        var maxTime = DateTime.Now.ToUniversalTime().Add(Interval);
        var query = base.BuildQuery(context);
        query = query.Where(mm => mm.GameTimeUTC <= maxTime);
        
        return query;
    }
}