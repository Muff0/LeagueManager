using Shared.Dto;
using Shared.Enum;

namespace Data.Commands.Match;

public class SetMatchesGameAnalysisStatusCommand : Command<LeagueContext>
{
    public MatchDto[] Matches { get; set; } = [];
    public GameAnalysisStatus NewStatus { get; set; } = GameAnalysisStatus.NotQueued;

    protected override void RunAction(LeagueContext context)
    {
        foreach (var currentMatch in Matches)
        {
            var existingMatch = context.Matches.FirstOrDefault(mm => mm.Id == currentMatch.Id);

            if (existingMatch == null)
                continue;

            existingMatch.GameAnalysisStatus = NewStatus;
        }
    }
}