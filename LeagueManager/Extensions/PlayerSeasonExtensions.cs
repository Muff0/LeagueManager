using Data.Model;
using LeagueManager.ViewModel;
using Shared.Dto;

namespace LeagueManager.Extensions;

public static class PlayerSeasonExtensions
{
    public static PlayerSeasonDto ToPlayerSeasonDto(this PlayerSeasonViewModel playerSeason)
    {
        return new PlayerSeasonDto()
        {
            PlayerId =  playerSeason.PlayerId,
            SeasonId =  playerSeason.SeasonId,
            PlayerParticipationTier = playerSeason.PlayerParticipationTier,
            PlayerPaymentStatus = playerSeason.PlayerPaymentStatus,
        };
    }

}