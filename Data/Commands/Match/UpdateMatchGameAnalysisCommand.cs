using Shared.Enum;

namespace Data.Commands.Match;

public class UpdateMatchGameAnalysisCommand : Command<LeagueContext>
{
    public int MatchId { get; set; }
    public string? GameAnalysisUrl { get; set; } = null;
    public GameAnalysisStatus? SetNewStatus { get; set; } = null;

    protected override void RunAction(LeagueContext context)
    {
        var existingMatch = context.Matches.FirstOrDefault(mm => mm.Id == MatchId);

        if (existingMatch == null)
            throw new ArgumentException($"Match with id {MatchId} does not exist");

        if (GameAnalysisUrl != null)
            existingMatch.GameAnalysisUrl = GameAnalysisUrl;
        if (SetNewStatus != null)
            existingMatch.GameAnalysisStatus = SetNewStatus.Value;
    }
}