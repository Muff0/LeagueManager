using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetMatchesByTimeQuery : Query<LeagueContext,Match>
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public bool IncludeCompleted { get; set; } = false;
        public bool InlcudePlayers { get; set; } = false;
        public bool IncludeNotConfirmed { get; set; } = false;
        public int MaxCount { get; set; } = 0;

        protected override IQueryable<Match> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (InlcudePlayers)
                query = query.Include(mm => mm.PlayerMatches)
                    .ThenInclude(pm => pm.Player);

            if (TimeFrom != null)
                query = query.Where(mm => mm.GameTimeUTC >= TimeFrom.GetValueOrDefault().ToUniversalTime());

            if (TimeTo != null)
                query = query.Where(mm => mm.GameTimeUTC <= TimeTo.GetValueOrDefault().ToUniversalTime());

            if (!IncludeNotConfirmed)
                query = query.Where(mm => mm.PlayerMatches.All(pm => pm.HasConfirmed));

            if (!IncludeCompleted)
                query = query.Where(mm => !mm.IsComplete);

            query = query.OrderBy(mm => mm.GameTimeUTC);

            if (MaxCount > 0)
                query = query.Take(MaxCount);

            return query;
        }

    }
}
