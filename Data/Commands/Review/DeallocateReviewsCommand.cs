using Shared.Dto;
using Shared.Enum;

namespace Data.Commands.Review;

public class DeallocateReviewsCommand : Command<LeagueContext>
{
    public ReviewDto[] Reviews { get; set; } = [];
    protected override void RunAction(LeagueContext context)
    {
        if ( Reviews == null )
            return;

        foreach (var review in Reviews)
        {
            var existingReview = context.Reviews.FirstOrDefault(r => r.Id == review.Id);
            if (existingReview == null)
                return;

            existingReview.TeacherId = null;
            existingReview.ReviewStatus = ReviewStatus.Planned;
        }
            
    }
}