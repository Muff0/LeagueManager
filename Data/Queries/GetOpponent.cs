using Data.Model;

namespace Data.Queries;

public class GetOpponentQuery :Scalar<LeagueContext,Player>
{
    public int PlayerId { get; set; }
    public int Round { get; set; }
    public int? SeasonId { get; set; }

    protected override IQueryable<Player> BuildQuery(LeagueContext context)
    {
        return base.BuildQuery(context)
            .Where(p => p.PlayerMatches
                .Any(pm => pm.Match.Round == Round
                           && (SeasonId == null ? pm.Match.Season.IsActive : pm.Match.Season.Id == SeasonId)
                           && pm.Match.PlayerMatches.Any(pm2 => pm2.PlayerId == PlayerId)
                           && pm.PlayerId != PlayerId));
    }
}