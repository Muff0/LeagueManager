using Shared.Dto;
using Shared.Enum;

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

                if (currentReview.Round != null)
                    existingReview.Round = (int)currentReview.Round;
                if(currentReview.ReviewStatus != null)
                    existingReview.ReviewStatus = (ReviewStatus)currentReview.ReviewStatus;

                if(currentReview.ReviewUrl != null)
                    existingReview.ReviewUrl = currentReview.ReviewUrl;

                if (currentReview.MatchId != 0)
                    existingReview.MatchId = currentReview.MatchId;

                context.Entry(existingReview).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }
        }
    }
}