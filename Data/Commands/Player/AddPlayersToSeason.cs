using Microsoft.EntityFrameworkCore;
using Shared.Dto;

namespace Data.Commands.Player
{
    public class AddPlayersToSeason : Command<LeagueContext>
    {
        public PlayerDto[] Players { get; set; } = [];
        public int SeasonId { get; set; } = 0;

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var player in Players)
            {
                var existingPlayer = context.Players.Include(pl => pl.PlayerSeasons)
                    .FirstOrDefault(pl => pl.Id == player.Id
                        || pl.LeagoKey == player.LeagoKey);

                if (existingPlayer == null)
                    continue;

                var existingPlayerSeason = existingPlayer.PlayerSeasons.FirstOrDefault(ps => ps.SeasonId == SeasonId);

                if (existingPlayerSeason == null)
                    existingPlayer.PlayerSeasons.Add(new Model.PlayerSeason()
                    {
                        SeasonId = SeasonId,
                    });
            }
        }
    }
}