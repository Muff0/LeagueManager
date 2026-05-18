using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries
{
    
    public class GetCommandMessagesQuery : Query<QueueContext, CommandMessage>
    {
        public string[] Types { get; set; } = [];
        protected override IQueryable<CommandMessage> BuildQuery(QueueContext context)
        {
            var query = base.BuildQuery(context);

            query = query.Where(cm => cm.Status == Shared.Enum.QueueStatus.Pending);
            
            if (Types.Length > 0)
                query = query.Where(cm => Types.Contains(cm.Type));
            
            return query;
        }
    }
}