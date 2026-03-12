using Data.Model;
using Shared.Dto;
using Shared.Enum;

namespace Data
{
    public static class MatchExtension
    {
        public static MatchDto ToMatchDto(this Match match)
        {
            return new MatchDto()
            {
                Id = match.Id,
                LeagoKey = match.LeagoKey,
                ScheduleTime = match.GameTimeUTC.GetValueOrDefault(),
                GameLink = match.MatchUrl,
                IsPlayed = match.IsComplete,
                Players = match.PlayerMatches?.Select(pm => pm.ToPlayerMatchDto(skipMatch: true)).ToArray(),
                Round = match.Round,
                SeasonId = match.SeasonId
            };
        }

        public static bool IsPlayed(this Match match)
            => match.PlayerMatches.Any(pm => pm.Outcome == Shared.Enum.PlayerMatchOutcome.Win);
    }
}