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

        public override IQueryable<Match> Execute(LeagueContext context)
        {
            var query = context.Matches.AsQueryable();
            if (InlcudePlayers)
                query = query.Include(mm => mm.PlayerMatches)
                    .ThenInclude(pm => pm.Player);

            query = query.Where(mm => mm.GameTimeUTC >= TimeFrom.GetValueOrDefault().ToUniversalTime()
                    && mm.GameTimeUTC <= TimeTo.GetValueOrDefault().ToUniversalTime());

            if (!IncludeCompleted)
                query = query.Where(mm => !mm.IsComplete);

            return query;
        }
    }
}
