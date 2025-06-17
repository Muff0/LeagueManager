using Data.Model;
using LeagueManager.ViewModel;

namespace LeagueManager.Extensions
{
    public static class ReviewExtensions
    {
        public static ReviewViewModel ToViewModel(this Review review)
        {
            var blackPlayer = review.Match?.PlayerMatches?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.Black)?.Player;
            var whitePlayer = review.Match?.PlayerMatches?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.White)?.Player;

            return new ReviewViewModel()
            {
                Id = review.Id,
                ReviewStatus = review.ReviewStatus,
                MatchId = review.MatchId,
                OwnerName = review.OwnerPlayer?.FirstName + " " + review.OwnerPlayer?.LastName,
                ReviewUrl = review.ReviewUrl,
                Round = review.Round,
                TeacherId = review.TeacherId,
                TeacherName = review.Teacher?.Name ?? string.Empty,
                BlackPlayerName = blackPlayer?.FirstName + " "+ blackPlayer?.LastName,
                WhitePlayerName = whitePlayer?.FirstName + " "+ whitePlayer?.LastName,
                BlackPlayerRank = blackPlayer?.Rank.ToString() ?? string.Empty,
                WhitePlayerRank = blackPlayer?.Rank.ToString() ?? string.Empty,
                MatchUrl = review.Match?.MatchUrl ?? string.Empty,
            };
        }
    }
}
