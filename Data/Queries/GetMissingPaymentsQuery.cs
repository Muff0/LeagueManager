using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    public class GetMissingPaymentsQuery : Query<LeagueContext, Player>
    {
        public int SeasonId { get; set; }

        protected override IQueryable<Player> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context)
                .Include(mm => mm.PlayerSeasons)
                .Where(pl => pl.PlayerSeasons.Any(
                    ps => ps.SeasonId == SeasonId && ps.PaymentStatus == Shared.Enum.PlayerPaymentStatus.None));

            return query;
        }
    }
}