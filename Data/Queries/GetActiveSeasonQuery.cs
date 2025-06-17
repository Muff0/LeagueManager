using Data.Model;

namespace Data.Queries
{
    public class GetNextCommandMessageQuery : Scalar<QueueContext, CommandMessage>
    {
        protected override IQueryable<CommandMessage> BuildQuery(QueueContext context)
        {
            var query = base.BuildQuery(context);

            query = query.Where(cm => cm.Status == Shared.Enum.QueueStatus.Pending);

            query = query.OrderBy(cm => cm.CreatedAtUtc);

            return query;
        }
    }
}