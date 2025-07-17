using Shared.Dto;

namespace Data.Commands.Review
{
    public class UpdateReviewsCommand : Command<LeagueContext>
    {
        public ReviewDto[] Reviews { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var currentReview in Reviews)
            {
                var existingReview = context.Reviews.FirstOrDefault(re => re.Id == currentReview.Id);

                if (existingReview == null)
                    throw new InvalidOperationException("Invalid Review Id");

                existingReview.Round = currentReview.Round;
                existingReview.ReviewStatus = currentReview.ReviewStatus;
                existingReview.ReviewUrl = currentReview.ReviewUrl ?? "";
            }
        }
    }
}