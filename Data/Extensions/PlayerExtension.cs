using Data.Model;
using Shared.Dto;

namespace Data
{
    public static class PlayerExtension
    {
        public static PlayerDto ToPlayerDto(this Player player)
        {
            return new PlayerDto()
            {
                Id = player.Id,
                LeagoMemberId = player.LeagoMemberId,
                OGSHandle = player.OGSHandle,
                DiscordHandle = player.DiscordHandle,
                EmailAddress = player.EmailAddress,
                FirstName = player.FirstName,
                LastName = player.LastName,
                LeagoKey = player.LeagoKey,
                Rank = player.Rank,
            };
        }
    }
}