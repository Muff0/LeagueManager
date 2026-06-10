using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetNextCommandMessageQuery : Scalar<QueueContext, CommandMessage>
{
    public string[] Types { get; set; } = [];

    protected override IQueryable<CommandMessage> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);

        query = query.Where(cm => cm.Status == QueueStatus.Pending);

        if (Types.Length > 0)
            query = query.Where(cm => Types.Contains(cm.Type));

        query = query.OrderBy(cm => cm.CreatedAtUtc);

        return query;
    }
}