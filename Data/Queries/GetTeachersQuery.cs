using Data.Model;

namespace Data.Queries;

public class GetTeachersQuery : Query<LeagueContext, Teacher>
{
    public override IQueryable<Teacher> BuildQuery(LeagueContext context)
    {
        return base.BuildQuery(context);
    }
}