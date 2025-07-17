using Shared.Dto;
using Shared.Enum;

namespace Shared
{
    public static class MatchDtoExtension
    {


        public static string BuildMatchTitle(this MatchDto match)
        {
            var whitePlayer = match.Players?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.White);
            var blackPlayer = match.Players?.FirstOrDefault(pm => pm.Color == Shared.Enum.PlayerColor.Black);
            if (whitePlayer?.Player == null || blackPlayer?.Player == null)
                return "";

            return string.Format(Templates.MatchTitleTemplate,
                match.Round,
                blackPlayer.Player.FirstName + " " + blackPlayer.Player.LastName,
                blackPlayer.Player.Rank.GetDisplayName(),
                whitePlayer.Player.FirstName + " " + whitePlayer.Player.LastName,
                whitePlayer.Player.Rank.GetDisplayName());
        }


    }
}
