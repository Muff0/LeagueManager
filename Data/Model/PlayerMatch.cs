using Shared.Enum;

namespace Data.Model
{
    public class PlayerMatch
    {
        public int PlayerId { get; set; }
        public int MatchId { get; set; }

        public Player? Player { get; set; }
        public Match? Match { get; set; }
        public PlayerColor Color { get; set; }
        public bool HasConfirmed { get; set; }
        public PlayerMatchOutcome Outcome { get; set; } = PlayerMatchOutcome.NotReported;
    }
}