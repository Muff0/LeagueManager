using Data.Model;
using Shared.Dto;

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
                GameLink = match.Link,
                IsPlayed = match.IsComplete,
                Players = match.PlayerMatches?.Select(pm => pm.ToPlayerMatchDto(skipMatch: true)).ToArray(),
                Round = match.Round,
                SeasonId = match.SeasonId
            };
        }
    }
}