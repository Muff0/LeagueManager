using Shared.Dto;

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