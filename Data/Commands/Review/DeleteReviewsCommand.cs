using Shared.Dto;

namespace Data.Commands.Review
{
    public class DeleteReviewsCommand : Command<LeagueContext>
    {
        public IEnumerable<ReviewDto> Reviews { get; set; }

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            if (Reviews == null)
                return;

            foreach (var review in Reviews)
            {
                var existingReview = context.Reviews.FirstOrDefault(re => re.Id == review.Id);
                if (existingReview != null)
                {
                    context.Remove(existingReview);
                }
            }
        }
    }
}
