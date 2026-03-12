using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetPlayersQuery : Query<LeagueContext, Player>
    {
        public bool IncludePlayerSeasons { get; set; } = false;
        public bool IncludePlayerMatches { get; set; } = false;

        public int SeasonId { get; set; } = 0;

        protected override IQueryable<Player> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (IncludePlayerSeasons)
                query = query.Include(mm => mm.PlayerSeasons);

            if (IncludePlayerMatches)
                query = query.Include(mm => mm.PlayerMatches);

            if (SeasonId != 0)
                query = query.Where(pl => pl.PlayerSeasons.Any(pl => pl.SeasonId == SeasonId));

            if (OrderResults)
                query = query.OrderBy(pl => pl.LastName).ThenBy(pl => pl.LastName);

            return query;
        }
    }
}