using Shared.Dto;

namespace Data.Commands.Player;

public class UpdatePlayerSeasons : Command<LeagueContext>
{
    public PlayerSeasonDto[] PlayerSeasons { get; set; } = [];

    protected override void RunAction(LeagueContext context)
    {
        base.RunAction(context);

        foreach (var player in PlayerSeasons)
        {
            var existingPlayerSeason = context.PlayerSeasons.FirstOrDefault(ps =>
                ps.PlayerId == player.PlayerId && ps.SeasonId == player.SeasonId);

            if (existingPlayerSeason == null)
                continue;

            if (player.PlayerParticipationTier != null)
                existingPlayerSeason.ParticipationTier = player.PlayerParticipationTier.Value;
            if(player.PlayerPaymentStatus!= null)
                existingPlayerSeason.PaymentStatus = player.PlayerPaymentStatus.Value;
        }
    }
}