using Data.Model;

namespace Data.Queries;

public class GetTeacherQuery : Query<LeagueContext, Teacher>
{
    public int Id { get; set; }

    public override IQueryable<Teacher> BuildQuery(LeagueContext context)
    {
        var query = base.BuildQuery(context);

        query = query.Where(x => x.Id == Id);

        return query;
    }
}