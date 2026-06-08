using System.ComponentModel;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Shared.Dto;

namespace Data.Commands.Match;

public class AddOrUpdateMatchesCommand : Command<LeagueContext>
{
    public MatchDto[] ToUpdate { get; set; } = [];
    public int SeasonId { get; set; }
    public int Round { get; set; }
    
    protected override void RunAction(LeagueContext context)
    {
        var toAdd = new List<Data.Model.Match>();
        foreach (var currentMatch in ToUpdate)
        {
            if (currentMatch.Players == null)
                throw new InvalidOperationException("Players is null");

            var existingMatch = context.Matches.Include(mm => mm.PlayerMatches)
                .ThenInclude(pm => pm.Player)
                .FirstOrDefault(mm => mm.LeagoKey == currentMatch.LeagoKey);

            if (existingMatch == null)
            {
                var newMatch = new Data.Model.Match()
                {
                    LeagoKey = currentMatch.LeagoKey,
                    SeasonId = SeasonId,
                    Round = Round,
                    MatchUrl = currentMatch.GameLink ?? "",
                    GameTimeUTC = currentMatch.ScheduleTime.GetValueOrDefault().ToUniversalTime(),
                    PlayerMatches = new List<PlayerMatch>()
                };

                foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                {
                    if (playerMatch?.Player == null)
                        continue;

                    var existingPlayer = context.Players.FirstOrDefault(pp => pp.LeagoKey == playerMatch.Player.LeagoKey);

                    if (existingPlayer == null)
                        continue;

                    var newPlayerMatch = new PlayerMatch()
                    {
                        Color = playerMatch.Color,
                        PlayerId = existingPlayer.Id,
                        HasConfirmed = playerMatch.HasConfirmed,
                        Outcome = playerMatch.Outcome,
                    };

                    newMatch.PlayerMatches.Add(newPlayerMatch);
                }

                toAdd.Add(newMatch);
            }
            else
            {
                existingMatch.GameTimeUTC = currentMatch.ScheduleTime.GetValueOrDefault().ToUniversalTime();
                existingMatch.MatchUrl = currentMatch.GameLink ?? "";

                existingMatch.IsComplete = currentMatch.Players.Any(pl => pl.Outcome != Shared.Enum.PlayerMatchOutcome.NotReported);

                foreach (PlayerMatchDto playerMatch in currentMatch.Players)
                {
                    var existingPlayerMatch = existingMatch.PlayerMatches?.FirstOrDefault(pm => pm.Player?.LeagoKey == playerMatch.Player?.LeagoKey);

                    if (existingPlayerMatch == null)
                    {
                        if (playerMatch?.Player == null)
                            continue;

                        var existingPlayer = context.Players.FirstOrDefault(pp => pp.LeagoKey == playerMatch.Player.LeagoKey);

                        if (existingPlayer == null)
                            continue;
                        existingPlayerMatch = new PlayerMatch()
                        {
                            Color = playerMatch.Color,
                            PlayerId = existingPlayer.Id,
                            HasConfirmed = playerMatch.HasConfirmed,
                            Outcome = playerMatch.Outcome,
                        };
                        existingMatch.PlayerMatches.Add(existingPlayerMatch);   
                    }

                    existingPlayerMatch.HasConfirmed = playerMatch.HasConfirmed;
                    existingPlayerMatch.Outcome = playerMatch.Outcome;
                    existingPlayerMatch.Color = playerMatch.Color;
                }
            } 
        }
        context.AddRange(toAdd);
    }
}