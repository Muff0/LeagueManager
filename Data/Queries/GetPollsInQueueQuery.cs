using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetPollsInQueueQuery : Query<QueueContext, Poll>
{
    public override IQueryable<Poll> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);

        query = query.Where(cm => cm.Status == QueueStatus.Pending);

        query = query.OrderBy(cm => cm.CreatedAtUtc);

        return query;
    }
}