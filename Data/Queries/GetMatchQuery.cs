using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetMatchQuery : Scalar<LeagueContext, Match>
    {
        public int Id { get; set; }
        public bool IncludePlayers { get; set; } = false;

        protected override IQueryable<Match> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (IncludePlayers)
                query = query.Include(ma => ma.PlayerMatches).ThenInclude(pm => pm.Player);

            return query.Where(ma => ma.Id == Id);
        }
    }
}
