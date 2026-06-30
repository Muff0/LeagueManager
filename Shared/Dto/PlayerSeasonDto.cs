using Shared.Enum;

namespace Shared.Dto;

public class PlayerSeasonDto
{
    public int PlayerId { get; set; }
    public int SeasonId { get; set; }
    public PlayerParticipationTier? PlayerParticipationTier { get; set; }
    public PlayerPaymentStatus? PlayerPaymentStatus { get; set; }
}