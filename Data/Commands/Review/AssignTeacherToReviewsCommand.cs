namespace Data.Commands.Review
{
    public class AssignTeacherToReviewsCommand : Command<LeagueContext>
    {
        public int TeacherId { get; set; }

        public int[] ReviewIds { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var reviewId in ReviewIds)
            {
                var existingReview = context.Reviews.FirstOrDefault(re => re.Id == reviewId);
                if (existingReview != null)
                {
                    existingReview.TeacherId = TeacherId;
                }
            }
        }
    }
}
