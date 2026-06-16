
using Shared;
using Shared.Dto;

namespace Mail.MessageBuilders;

public class GameAnalysisReadyMessage
{
    
    public GameAnalysisReadyMessage(MatchDto match)
    {
        
        Subject = $"Go Magic League - Round {match.Round} - Your AI review by Kifubara!";
        HtmlBody = $"""
                    <h2>Your AI analysis for this game is ready!</h2>
                    <h3>{match.BuildMatchTitle()}</h3>
                    <br/>
                    <p> Kifubara is offering all League players a free analysis at their highest tier: 2500 playouts with the latest KataGo model!</p>
                    <p> To access it, simply follow this link: </p> <a href="{match.GameAnalysisUrl}">Your Kifubara review</a>.
                    <p>

                    <p> Thank you, </p>
                    """;
    }

    public string Subject { get; }
    public string HtmlBody { get; }
}