using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class QueueDataService : DataServiceBase<QueueContext>
    {
        public QueueDataService(IDbContextFactory<QueueContext> contextFactory) : base(contextFactory)
        {
        }
    }
}