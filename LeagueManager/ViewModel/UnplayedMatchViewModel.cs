using Shared.Enum; 

namespace LeagueManager.ViewModel;

/// <summary>
/// View model for a single unplayed match row on the Matches page.
/// Carries UI-only selection state (Player1Forfeit / Player2Forfeit) alongside
/// the data needed to render the row and its detail view.
/// </summary>
public class UnplayedMatchViewModel
{
    public int MatchId { get; set; }
    public int Round { get; set; }

    public UnplayedMatchPlayerViewModel Player1 { get; set; } = new();
    public UnplayedMatchPlayerViewModel Player2 { get; set; } = new();

    public DateTime? ScheduledTime { get; set; }

    // --- UI-only selection state, not persisted server-side until submitted ---
    public bool Player1Forfeit { get; set; }
    public bool Player2Forfeit { get; set; }

    public bool HasAnyForfeit => Player1Forfeit || Player2Forfeit;
}

public class UnplayedMatchPlayerViewModel
{
    public int PlayerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public PlayerRank Rank { get; set; }

    public string? OgsUsername { get; set; }
    public string? DiscordHandle { get; set; }

    /// <summary>Whether this player has confirmed the ScheduledTime on the parent match.</summary>
    public bool HasConfirmedTime { get; set; }
}
