namespace Data.Commands.Match;

public class SetMatchGameAnalysisUrlCommand : Command<LeagueContext>
{
    public int MatchId { get; set; }
    public string GameAnalysisUrl { get; set; }

    protected override void RunAction(LeagueContext context)
    {
        var existingMatch = context.Matches.FirstOrDefault(mm => mm.Id == MatchId);

        if (existingMatch == null)
            throw new ArgumentException($"Match with id {MatchId} does not exist");

        existingMatch.GameAnalysisUrl = GameAnalysisUrl;
    }
}