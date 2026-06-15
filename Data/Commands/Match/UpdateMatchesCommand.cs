using Microsoft.EntityFrameworkCore;
using Shared.Dto;
using Shared.Enum;

namespace Data.Commands.Match;

public class UpdateMatchesCommand : Command<LeagueContext>
{
    public MatchDto[] ToUpdate { get; set; } = [];

    protected override void RunAction(LeagueContext context)
    {
        foreach (var currentMatch in ToUpdate)
        {
            if (currentMatch.Players == null)
                throw new InvalidOperationException("Players is null");

            var existingMatch = context.Matches.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .FirstOrDefault(mm => mm.LeagoKey == currentMatch.LeagoKey);

            if (existingMatch == null)
                continue;

            existingMatch.GameTimeUTC = currentMatch.ScheduleTime.GetValueOrDefault().ToUniversalTime();
            existingMatch.IsComplete = currentMatch.Players.Any(pl => pl.Outcome != PlayerMatchOutcome.NotReported);
            existingMatch.OgsLeagueMatchId = currentMatch.OgsLeagueMatchId;
            
            foreach (var playerMatch in currentMatch.Players)
            {
                var existingPlayerMatch =
                    existingMatch.PlayerMatches?.FirstOrDefault(pm =>
                        pm.Player?.LeagoKey == playerMatch.Player?.LeagoKey);

                if (existingPlayerMatch == null)
                    continue;

                existingPlayerMatch.HasConfirmed = playerMatch.HasConfirmed;
                existingPlayerMatch.Outcome = playerMatch.Outcome;
                existingPlayerMatch.Color = playerMatch.Color;
            }
        }
    }
}