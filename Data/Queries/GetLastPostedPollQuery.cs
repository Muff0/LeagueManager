using Data.Model;

namespace Data.Queries
{
    
    public class GetLastPostedPollQuery : Scalar<QueueContext, Poll>
    {
        protected override IQueryable<Poll> BuildQuery(QueueContext context)
        {
            var query = base.BuildQuery(context);

            query = query.Where(cm => cm.Status == Shared.Enum.QueueStatus.Completed);
            
            query = query.OrderByDescending(cm => cm.ProcessedAtUtc);

            return query;
        }
    }
}