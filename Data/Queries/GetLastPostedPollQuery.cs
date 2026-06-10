using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetLastPostedPollQuery : Scalar<QueueContext, Poll>
{
    protected override IQueryable<Poll> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);

        query = query.Where(cm => cm.Status == QueueStatus.Completed
                                  && cm.ProcessedAtUtc != null);

        query = query.OrderByDescending(cm => cm.ProcessedAtUtc);

        return query;
    }
}