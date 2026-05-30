using Data;
using Data.Extensions;
using Data.Model;
using Data.Queries;
using Mail;
using Mail.MessageBuilders;

namespace LeagueCoreService.Queue;

public class SendUnconfirmedMatchRemindersHandler(MailService mailService,
    LeagueDataService leagueDataService) : ICommandHandler
{
    public string CommandType => "SendUnconfirmedMatchReminders";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var season = (await leagueDataService.RunQueryAsync(new GetActiveSeasonQuery()))
            .ToSeasonDto();
        var unconfirmedMatches = (await leagueDataService.RunQueryAsync(
            new GetUnconfirmedMatchesQuery()
            {
                IncludePlayers = true,
                SeasonId = season.Id
            })).Select(m => m.ToMatchDto()).ToArray();

        foreach (var currentMatch in unconfirmedMatches)
        {
            var pl = currentMatch.Players.FirstOrDefault(pl => pl.Player.FirstName == "Pietro")?.Player;
            if (pl == null)
                continue;
            var msg = new MatchReminderMessage(currentMatch, NextMondayNoon(), season);
            await mailService.SendAsync(pl.EmailAddress, pl.FirstName, msg.Subject, msg.HtmlBody);
        }
    }
    public DateTime NextMondayNoon()
    {
        var today = DateTime.UtcNow.Date;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7; // today is Monday → next Monday
        return today.AddDays(daysUntilMonday).AddHours(12);
    }
}