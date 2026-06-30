
using Shared;
using Shared.Dto;
using Shared.Enum;

namespace Mail.MessageBuilders;

public class PostedReviewMessage
{
    
    public PostedReviewMessage(ReviewDto reviewDto, MatchDto match, TeacherDto teacher)
    {
        
        Subject = $"Go Magic League - Round {match.Round} - Your Video Review";
        HtmlBody = $"""
                    <h2>Your video Review for this game is ready:</h2>
                    <h3>{match.BuildMatchTitle()}</h3>
                    <br/>
                    <p> To see it, simply follow this link: </p> <a href="{reviewDto.ReviewUrl}">Your video Review</a>
                    <p> Teacher: {BuildTeacherString(teacher)}</p>
                    <p>
                    <p> Visit our Discord Server for more League Reviews and to ask any question to our teachers: </p>
                    <p> <a href="https://discord.com/channels/731050648317198366/1328423818938548254">Discord Review Archive</a></p>
                    <p> Thank you, </p>
                    """;
    }

    public string Subject { get; }
    public string HtmlBody { get; }
    
    
    private string BuildTeacherString(TeacherDto teacher)
    {
        return teacher.Name + " " + teacher.Rank.GetDisplayName();
    }
}