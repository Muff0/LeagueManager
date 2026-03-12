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
                    || pl.LeagoKey == player.LeagoKey);

                if(existingPlayer == null)
                {
                    var sameName = context.Players.Where(pl => pl.FirstName == player.FirstName && pl.LastName == player.LastName).ToArray();
                    if (sameName.Length == 1)
                        existingPlayer = sameName[0];
                }

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
                        Timezone = player.TimeZone ?? string.Empty,
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
                    if (player.DiscordId != null)
                        existingPlayer.DiscordId = player.DiscordId;
                    if (player.TimeZone != null)
                        existingPlayer.Timezone = player.TimeZone;

                    context.Entry(existingPlayer).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }
        }
    }
}