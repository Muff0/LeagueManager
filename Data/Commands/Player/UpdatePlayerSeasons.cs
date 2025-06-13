using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Commands;
using Shared.Dto;
using Shared.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Data.Commands.Player
{
    public class UpdatePlayerSeasons : Command<LeagueContext>
    {
        public PlayerRegistrationDto[] PlayerRegistrations { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var player in PlayerRegistrations)
            {
                var existingPlayerSeason = context.PlayerSeasons.FirstOrDefault(ps => ps.PlayerId == player.PlayerId && ps.SeasonId == player.SeasonId);

                if (existingPlayerSeason == null)
                    continue;

                existingPlayerSeason.ParticipationTier = player.PlayerParticipationTier;
                existingPlayerSeason.PaymentStatus = player.PlayerPaymentStatus;
            }
        }
    }
}
