using Data.Model;
using LeagueManager.ViewModel;
using Shared.Dto;

namespace LeagueManager.Extensions
{
    public static class PlayerExtensions
    {
        public static PlayerViewModel ToPlayerViewModel(this Player player) => new PlayerViewModel()
        {
            Id = player.Id,
            LeagoMemberId = player.LeagoMemberId,
            OGSHandle = player.OGSHandle,
            DiscordHandle = player.DiscordHandle,
            DiscordId = player.DiscordId,
            EmailAddress = player.EmailAddress,
            FirstName = player.FirstName,
            LastName = player.LastName,
            LeagoKey = player.LeagoKey,
            Rank = player.Rank,
        };
        public static PlayerDto ToPlayerDto(this PlayerViewModel player)
        {
            return new PlayerDto()
            {
                Id = player.Id,
                LeagoMemberId = player.LeagoMemberId,
                OGSHandle = player.OGSHandle,
                DiscordHandle = player.DiscordHandle,
                DiscordId = player.DiscordId,
                EmailAddress = player.EmailAddress,
                FirstName = player.FirstName,
                LastName = player.LastName,
                LeagoKey = player.LeagoKey,
                Rank = player.Rank,
            };
        }
    }
}
