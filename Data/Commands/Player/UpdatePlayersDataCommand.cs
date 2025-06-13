using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Commands;
using Shared.Dto;

namespace Data.Commands.Player
{
    public class UpdatePlayersDataCommand : Command<LeagueContext>
    {
        public PlayerDto[] Players { get; set; } = [];

        protected override void RunAction(LeagueContext context)
        {
            base.RunAction(context);

            foreach (var player in Players)
            {
                var existingPlayer = context.Players.FirstOrDefault(pl => pl.Id == player.Id
                    || pl.LeagoKey == player.LeagoKey
                    || (pl.FirstName == player.FirstName && pl.LastName == player.LastName));


                if (existingPlayer == null)
                {
                    existingPlayer = new Data.Model.Player()
                    {
                        FirstName = player.FirstName ?? string.Empty,
                        LastName = player.LastName ?? string.Empty,
                        OGSHandle = player.OGSHandle ?? string.Empty,
                        DiscordHandle = player.DiscordHandle ?? string.Empty,
                        LeagoMemberId = player.LeagoMemberId ?? string.Empty,
                        LeagoKey = player.LeagoKey ?? string.Empty,
                        Rank = player.Rank,
                        EmailAddress = player.EmailAddress ?? string.Empty,
                    };
                    context.Players.Add(existingPlayer);
                }
                else
                {
                    if (player.Rank != Shared.Enum.PlayerRank.MinValue) 
                        existingPlayer.Rank = player.Rank;                    
                    if (player.OGSHandle != null)
                        existingPlayer.OGSHandle = player.OGSHandle;
                    if (player.LeagoMemberId != null)
                        existingPlayer.LeagoMemberId = player.LeagoMemberId;
                    if (player.LeagoKey != null)
                        existingPlayer.LeagoKey = player.LeagoKey;
                    if (player.DiscordHandle != null)
                        existingPlayer.DiscordHandle = player.DiscordHandle;
                    if (player.EmailAddress != null)
                        existingPlayer.EmailAddress = player.EmailAddress;
                    if (player.GoMagicUserId != null)
                        existingPlayer.GoMagicUserId = (int)player.GoMagicUserId;

                    context.Entry(existingPlayer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }
        }
    }
}
