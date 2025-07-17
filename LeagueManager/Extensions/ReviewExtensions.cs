using Data.Model;
using LeagueManager.ViewModel;
using Shared.Dto;
using Shared.Enum;

namespace LeagueManager.Extensions
{
    public static class ReviewExtensions
    {

        public static string BuildLongMatchTitle(this ReviewViewModel match)
        {
            return string.Format(Shared.Templates.LongMatchTitleTemplate,
                match.SeasonName,
                match.Round,
                match.BlackPlayerName,
                match.BlackPlayerRank,
                match.WhitePlayerName,
                match.WhitePlayerRank);
        }
        public static string BuildMatchTitle(this ReviewViewModel match)
        {
            return string.Format(Shared.Templates.MatchTitleTemplate,
                match.Round,
                match.BlackPlayerName,
                match.BlackPlayerRank,
                match.WhitePlayerName,
                match.WhitePlayerRank);
        }


        public static ReviewDto ToReviewDto(this ReviewViewModel reviewViewModel) => new ReviewDto()
        {
            Id = reviewViewModel.Id,
            ReviewStatus = reviewViewModel.ReviewStatus,
            MatchId = reviewViewModel.MatchId,
            ReviewUrl = reviewViewModel.ReviewUrl,
            Round = reviewViewModel.Round,
            TeacherId = reviewViewModel.TeacherId,
        };

        public static ReviewViewModel ToViewModel(this Review review)
        {
            var blackPlayer = review.Match?.PlayerMatches?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.Black)?.Player;
            var whitePlayer = review.Match?.PlayerMatches?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.White)?.Player;

            return new ReviewViewModel()
            {
                Id = review.Id,
                SeasonName = "Season 8",
                ReviewStatus = review.ReviewStatus,
                MatchId = review.MatchId,
                OwnerName = review.OwnerPlayer?.FirstName + " " + review.OwnerPlayer?.LastName,
                OwnerHandle = review.OwnerPlayer?.OGSHandle ?? "",
                ReviewUrl = review.ReviewUrl,
                Round = review.Round,
                TeacherId = review.TeacherId,
                TeacherName = review.Teacher?.Name ?? string.Empty,
                BlackPlayerName = blackPlayer?.FirstName + " " + blackPlayer?.LastName,
                WhitePlayerName = whitePlayer?.FirstName + " " + whitePlayer?.LastName,
                BlackPlayerRank = blackPlayer?.Rank.GetDisplayName() ?? string.Empty,
                WhitePlayerRank = whitePlayer?.Rank.GetDisplayName() ?? string.Empty,
                MatchUrl = review.Match?.MatchUrl ?? string.Empty,
                IsPlayed = review.Match?.IsComplete ?? false,
                MatchTime = review.Match?.GameTimeUTC ?? DateTime.MinValue,
            };
        }
    }
}
