using Shared.Dto;

namespace Mail.MessageBuilders;

public class RoundStartMessage
{
    public RoundStartMessage(SeasonDto season, int round, DateTime deadlineUtc)

    {
        Subject = $"Go Magic League {season.Title} - Round {round} Started";
        HtmlBody = $"""
                    <h2>{season.Title} -  Round {round} Started!</h2>
                    <br/>
                    <p> Hello players, Round {round} of {season.Title} has just begun!</p>
                    <p> You can see your pairings in the "You" page on Leago here: </p>
                    <a href="https://leago.gg/GoMagic/league/gomagic/s/{season.LeagoL1Key}/you">Pairings</a>
                    <br/>
                    <p> All matches in the current round will have to be played by 
                    {deadlineUtc.ToLongDateString()} at {deadlineUtc.ToShortTimeString()}.

                    If you're having trouble scheduling or contacting your opponent please reply to this email or contact me on Discord (username muff0).</p>

                    <strong>An unplayed match will be treated as forfeit unless you contact me.</strong>

                    <p> Thank you and have good games!</p>
                    """;
    }

    public string Subject { get; }
    public string HtmlBody { get; }
}