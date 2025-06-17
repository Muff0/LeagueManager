using Shared.Enum;

namespace Data.Model
{
    public class PlayerSeason
    {
        public int PlayerId { get; set; }
        public int SeasonId { get; set; }

        public Season? Season { get; set; }

        public Player? Player { get; set; }

        public ICollection<Match>? Matches { get; set; }

        public PlayerParticipationTier ParticipationTier { get; set; } = PlayerParticipationTier.None;
        public PlayerPaymentStatus PaymentStatus { get; set; } = PlayerPaymentStatus.None;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}