using System.Text.RegularExpressions;
using Shared;
using Shared.Dto;

namespace Mail.MessageBuilders;

public class MatchReminderMessage
{
    public string Subject { get; }
    public string HtmlBody { get; }

    public MatchReminderMessage(MatchDto match, DateTime deadlineUTC, SeasonDto season)
    {
        Subject = $"Go Magic League {season.Title} - Round {match.Round} - Match Reminder";
        HtmlBody = $"""
                    <h2>Upcoming Match Reminder</h2>
                    <h3>{match.BuildMatchTitle()}</h3>
                    <br/>
                    <p> Hello, your match for the current round doesn't have a confirmed date yet.</p>
                    <p> As a reminder, all matches in the current round will have to be played by 
                    {deadlineUTC.DayOfWeek} {deadlineUTC.ToShortDateString()} at {deadlineUTC.ToShortTimeString()}.
                    
                    If you're having trouble scheduling please reply to this email or contact me on Discord (username muff0).</p>
                    
                    <strong>An unplayed match will be treated as forfeit unless you contact me.</strong>
                    
                    <p> Thank you, </p>
                    """;
    }
    
    
}