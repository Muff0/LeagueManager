namespace Shared.Dto
{
    public class MatchDto
    {
        public int Id { get; set; }
        public string LeagoKey { get; set; } = string.Empty;
        public PlayerMatchDto[]? Players { get; set; } = Array.Empty<PlayerMatchDto>();

        public DateTime? ScheduleTime { get; set; }
        public string GameLink { get; set; } = string.Empty;
        public bool IsPlayed { get; set; } = false;
        public bool WhiteConfirmed { get; set; } = false;
        public bool BlackConfirmed { get; set; } = false;
        public int? MatchSetLevel { get; set; }

        public int Round { get; set; }
        public int SeasonId { get; set; }
        public SeasonDto? Season { get; set; }
    }
}