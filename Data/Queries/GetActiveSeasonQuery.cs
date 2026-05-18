using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetActiveSeasonQuery : Scalar<LeagueContext, Season>
    {
        public bool IncludePlayerSeasons { get; set; } = false;

        protected override IQueryable<Season> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            if (IncludePlayerSeasons)
                query = query.Include(x => x.PlayerSeasons);

            return query.Where(se => se.IsActive).OrderByDescending(se => se.Id);
        }
    }
}