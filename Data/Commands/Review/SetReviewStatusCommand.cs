using Shared.Dto;
using Shared.Enum;

namespace Data.Commands.Review
{
    public class SetReviewStatusCommand : Command<LeagueContext>
    {
        public ReviewStatus NewStatus { get; set; }
        public IEnumerable<ReviewDto> ReviewList { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var currentReview in ReviewList)
            {
                var existingReview = context.Reviews.FirstOrDefault(re => re.Id == currentReview.Id);

                if (existingReview == null)
                    continue;

                existingReview.ReviewStatus = NewStatus;
            }
        }
    }
}