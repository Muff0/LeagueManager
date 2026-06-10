namespace Data.Commands.Review;

public class AddReviewToPlayerCommand : Command<LeagueContext>
{
    public int PlayerId { get; set; }
    public int SeasonId { get; set; }

    protected override void RunAction(LeagueContext context)
    {
        base.RunAction(context);

        var existingPlayer = context.Players.FirstOrDefault(pl => pl.Id == PlayerId);

        if (existingPlayer != null)
            context.Reviews.Add(
                new Model.Review
                {
                    OwnerPlayerId = existingPlayer.Id,
                    SeasonId = SeasonId,
                    Round = 0
                });
    }
}