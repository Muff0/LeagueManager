using Data.Model;
using Shared.Enum;

namespace Data.Queries;

public class GetNextGameAnalysisQuery : Scalar<QueueContext, GameAnalysis>
{
    protected override IQueryable<GameAnalysis> BuildQuery(QueueContext context)
    {
        var query = base.BuildQuery(context);
        
        query = query.Where(cm => cm.Status == QueueStatus.Pending);
        query = query.OrderBy(cm => cm.CreatedAtUtc);

        return query;
    }

}