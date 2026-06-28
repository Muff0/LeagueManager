using Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Queries;

public class GetMatchesByTimeQuery : Query<LeagueContext, Match>
{
    public DateTime? TimeFromUtc { get; set; }
    public DateTime? TimeToUtc { get; set; }
    public bool IncludeCompleted { get; set; } = false;
    public bool InlcudePlayers { get; set; } = false;
    public bool IncludeNotConfirmed { get; set; } = false;

    /// <summary>
    ///     If a Value is provided the query is filtered by the NotificationSent field
    ///     to only include results with the given value
    /// </summary>
    public bool? IsNotificationSent { get; set; } = null;

    public override IQueryable<Match> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);

        if (InlcudePlayers)
            query = query.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player);

        if (TimeFromUtc != null)
            query = query.Where(mm => mm.GameTimeUTC >= TimeFromUtc.GetValueOrDefault());

        if (TimeToUtc != null)
            query = query.Where(mm => mm.GameTimeUTC <= TimeToUtc.GetValueOrDefault());

        if (!IncludeNotConfirmed)
            query = query.Where(mm => mm.PlayerMatches.All(pm => pm.HasConfirmed));

        if (!IncludeCompleted)
            query = query.Where(mm => !mm.IsComplete);

        if (IsNotificationSent != null)
            query = query.Where(mm => mm.NotificationSent == IsNotificationSent);

        query = query.OrderBy(mm => mm.GameTimeUTC);

        return query;
    }
}