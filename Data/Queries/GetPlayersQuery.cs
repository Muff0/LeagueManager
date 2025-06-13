using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetPlayersQuery : Query<LeagueContext,Player>
    {
        public bool IncludePlayerSeasons { get; set; } = false;
        public bool IncludePlayerMatches { get; set; } = false;

        public int SeasonId { get; set; } = 0;

        protected override IQueryable<Player> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (IncludePlayerSeasons)
                query = query.Include(mm => mm.PlayerSeasons)
                    .ThenInclude(pm => pm.Player);

            if (IncludePlayerMatches)
                query = query.Include(mm => mm.PlayerMatches)
                    .ThenInclude(pm => pm.Player);

            if (SeasonId != 0)
                query = query.Where(pl => pl.PlayerSeasons.Any(pl => pl.SeasonId == SeasonId));

            return query;
        }

    }
}
