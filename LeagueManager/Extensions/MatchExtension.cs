using Data.Model;
using LeagueManager.ViewModel;
using Shared.Enum;

namespace LeagueManager.Extensions;

public static class MatchExtension
{
    public static UnplayedMatchViewModel ToUnplayedMatchViewModel(this Match match)
    {
        var bPlayer = match.PlayerMatches.FirstOrDefault(pm => pm.Color == PlayerColor.Black);
        var wPlayer = match.PlayerMatches.FirstOrDefault(pm => pm.Color == PlayerColor.White);
        
        return new UnplayedMatchViewModel()
        {
            MatchId = match.Id,
            Round = match.Round
        };
    }
}