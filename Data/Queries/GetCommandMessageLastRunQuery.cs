using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetCommandMessageLastRunQuery : Query<QueueContext, CommandMessage>
{
    public string CommandMessageType { get; set; } = string.Empty;

    public override IQueryable<CommandMessage> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);

        query = query.Where(cm => cm.Status == QueueStatus.Completed
                                  && cm.Type == CommandMessageType);

        query = query.OrderByDescending(cm => cm.ProcessedAtUtc);

        return query;
    }
}