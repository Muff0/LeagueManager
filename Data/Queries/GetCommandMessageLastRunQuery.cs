using Data.Model;

namespace Data.Queries;

public class GetCommandMessageLastRunQuery : Scalar<QueueContext, CommandMessage>
{
    public string CommandMessageType { get; set; } = string.Empty;
        protected override IQueryable<CommandMessage> BuildQuery(QueueContext context)
        {
            var query = base.BuildQuery(context);

            query = query.Where(cm => cm.Status == Shared.Enum.QueueStatus.Completed
                && cm.Type == CommandMessageType);
            
            query = query.OrderByDescending(cm => cm.ProcessedAtUtc);

            return query;
        }
    
}