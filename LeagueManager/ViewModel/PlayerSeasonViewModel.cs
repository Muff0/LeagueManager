using Shared.Enum;

namespace LeagueManager.ViewModel;

public class PlayerSeasonViewModel
{
    public int PlayerId { get; set; }
    public int SeasonId { get; set; }
    public string SeasonTitle { get; set; } = string.Empty;
    public PlayerParticipationTier PlayerParticipationTier { get; set; }
}