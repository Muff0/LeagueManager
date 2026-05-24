using Shared.Dto;

namespace Data.Commands.Match;

public class SetMatchesNotifiedCommand : Command<LeagueContext>
{
    public MatchDto[] Matches { get; set; } = [];

    protected override void RunAction(LeagueContext context)
    {
        foreach (var currentMatch in Matches)
        {
            var existingMatch = context.Matches.FirstOrDefault(mm => mm.Id == currentMatch.Id);

            if (existingMatch == null)
                continue;

            existingMatch.NotificationSent = true;

        }
    }
}