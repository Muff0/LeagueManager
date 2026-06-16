using Data.Extensions;
using Data.Model;
using Shared.Dto;
using Shared.Enum;

namespace Data;

public static class MatchExtension
{
    public static MatchDto ToMatchDto(this Match match)
    {
        return new MatchDto
        {
            Id = match.Id,
            LeagoKey = match.LeagoKey,
            ScheduleTime = match.GameTimeUTC.GetValueOrDefault(),
            OgsLeagueMatchId =  match.OgsLeagueMatchId,
            IsPlayed = match.IsComplete,
            Players = match.PlayerMatches?.Select(pm => pm.ToPlayerMatchDto(true)).ToArray(),
            Round = match.Round,
            SeasonId = match.SeasonId,
            GameAnalysisUrl =  match.GameAnalysisUrl,
            Season = match.Season?.ToSeasonDto()
        };
    }

    public static string GetMatchUrl(this MatchDto dto) =>
        $"""https://online-go.com/online-league/league-game/{dto.OgsLeagueMatchId}""";
    
    public static bool IsPlayed(this Match match)
    {
        return match.PlayerMatches.Any(pm => pm.Outcome == PlayerMatchOutcome.Win);
    }
}