using Data.Model;
using Shared.Dto;
using Shared.Enum;

namespace Data
{
    public static class ReviewExtensions
    {



        public static ReviewDto ToReviewDto(this Review review) => new ReviewDto()
        {
            Id = review.Id,
            ReviewStatus = review.ReviewStatus,
            MatchId = review.MatchId,
            ReviewUrl = review.ReviewUrl,
            Round = review.Round,
            TeacherId = review.TeacherId,
            SeasonId = review.PlayerSeason?.SeasonId,
            SeasonTitle = review.PlayerSeason?.Season?.Title
        };
    }
}
