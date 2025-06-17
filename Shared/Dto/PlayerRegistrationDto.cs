using Shared.Enum;

namespace Shared.Dto
{
    public class PlayerRegistrationDto
    {
        public int PlayerId { get; set; }
        public int SeasonId { get; set; }
        public PlayerParticipationTier PlayerParticipationTier { get; set; }
        public PlayerPaymentStatus PlayerPaymentStatus { get; set; }
        public DateTime DateTime { get; set; }
    }
}