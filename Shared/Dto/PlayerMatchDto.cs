using Shared.Enum;

namespace Shared.Dto
{
    public class PlayerMatchDto
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
        public PlayerDto? Player { get; set; }
        public MatchDto? Match { get; set; }
        public PlayerColor Color { get; set; }
        public bool HasConfirmed { get; set; }
        public bool HasForfeited { get; set; }
        public PlayerMatchOutcome Outcome { get; set; }
    }
}