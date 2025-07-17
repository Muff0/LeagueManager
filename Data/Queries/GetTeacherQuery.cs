using Data.Model;

namespace Data.Queries
{
    public class GetTeacherQuery : Scalar<LeagueContext, Teacher>
    {

        public int Id { get; set; }

        protected override IQueryable<Teacher> BuildQuery(LeagueContext context)
        {
            var query = base.BuildQuery(context);

            query = query.Where(x => x.Id == Id);

            return query;
        }
    }
}
