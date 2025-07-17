using Shared.Enum;

namespace LeagueManager.ViewModel
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public int Round { get; set; }
        public int? TeacherId { get; set; }
        public ReviewStatus ReviewStatus { get; set; }
        public int? MatchId { get; set; }
        public string? ReviewUrl { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string TeacherName { get; set; } = string.Empty;
        public string MatchUrl { get; set; } = string.Empty;
        public string WhitePlayerName { get; set; } = string.Empty;
        public string BlackPlayerName { get; set; } = string.Empty;
        public string WhitePlayerRank { get; set; } = string.Empty;
        public string BlackPlayerRank { get; set; } = string.Empty;
        public bool IsPlayed { get; set; } = false;
        public DateTime MatchTime { get; set; } = DateTime.MinValue;
        public string OwnerHandle { get; internal set; } = string.Empty;
    }
}
