using Data.Model;

namespace Data.Queries;

public class GetPlayersForRoundQuery : Query<LeagueContext, Player>
{
    public int SeasonId { get; set; }
    public int Round { get; set; }

    public override IQueryable<Player> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);
        return query.Where(pl => pl.PlayerMatches.Any(pm => pm.Match.SeasonId == SeasonId
                                                            && pm.Match.Round == Round));
    }
}