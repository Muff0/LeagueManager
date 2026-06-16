using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetGameAnalysisQuery : Query<QueueContext, GameAnalysis>
{
    public bool OrderByCreatedAt { get; set; }
    public QueueStatus? WithStatus { get; set; } = null;
    
    public override IQueryable<GameAnalysis> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);
        
        if (WithStatus != null)
            query = query.Where(cm => cm.Status == WithStatus.Value);
        
        if (OrderByCreatedAt)
            query = query.OrderBy(cm => cm.CreatedAtUtc);

        return query;
    }
}