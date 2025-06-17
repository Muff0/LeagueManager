using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class LeagueDataService : DataServiceBase<LeagueContext>
    {
        public LeagueDataService(IDbContextFactory<LeagueContext> contextFactory) : base(contextFactory)
        {
        }
    }
}