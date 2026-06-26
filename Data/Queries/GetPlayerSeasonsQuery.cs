using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;

namespace Data.Queries;

public class GetPlayerSeasonsQuery : Query<LeagueContext, PlayerSeason>
{
    public bool IncludePlayer { get; set; } = false;
    public bool IncludeSeason { get; set; } = false;

    public int PlayerId { get; set; } = 0;
    public int SeasonId { get; set; } = 0;
    public PlayerParticipationTier[] ParticipationTiers { get; set; } = [];
    public bool IncludeReviews { get; set; }

    public override IQueryable<PlayerSeason> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);

        if (IncludePlayer)
            query = query.Include(pm => pm.Player);

        if (IncludeSeason)
            query = query.Include(mm => mm.Season);

        if (IncludeReviews)
            query = query.Include(mm => mm.Reviews);

        if (SeasonId != 0)
            query = query.Where(ps => ps.SeasonId == SeasonId);

        if (ParticipationTiers.Length > 0)
            query = query.Where(ps => ParticipationTiers.Contains(ps.ParticipationTier));
        
        if (PlayerId != 0)
            query = query.Where(ps => ps.PlayerId == PlayerId);

        return query;
    }
}