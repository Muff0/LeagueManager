using Data;
using Data.Commands.Queue;
using Data.Extensions;
using Data.Model;
using Data.Queries;
using Mail;
using Mail.MessageBuilders;
using Shared.Extensions;
using Shared.Queue;

namespace LeagueCoreService.Queue;

public class SendUnconfirmedMatchRemindersHandler(QueueDataService queueDataService,
    LeagueDataService leagueDataService) : ICommandHandler
{
    public string CommandType => "SendUnconfirmedMatchReminders";
    public async Task HandleAsync(CommandMessage cmd)
    {
        var season = (await leagueDataService.RunQueryAsync(new GetActiveSeasonQuery()))!
            .ToSeasonDto();
        var unconfirmedMatches = (await leagueDataService.RunQueryAsync(
            new GetUnconfirmedMatchesQuery()
            {
                IncludePlayers = true,
                SeasonId = season.Id
            })).Select(m => m.ToMatchDto()).ToArray();
        
        foreach (var currentMatch in unconfirmedMatches)
        {
            var msg = new MatchReminderMessage(currentMatch, NextMondayNoon(), season);
            foreach (var player in currentMatch.Players!)
            {
                if (player?.Player?.EmailAddress == null)
                    continue;
                var payload = new SendEmailPayload()
                {
                    HtmlBody = msg.HtmlBody,
                    Subject = msg.Subject,
                    ToAddress = player.Player.EmailAddress,
                    ToName = player.Player.FirstName! + " " + player.Player.LastName!,
                    Ccs = ["league@gomagic.org"]
                };
                await queueDataService.ExecuteAsync(
                    new InsertCommandMessageCommand()
                    {
                        NewCommand = new CommandMessage()
                        {
                            Type = "SendEmail",
                            Payload = payload.SerializePayload()
                        }
                    });
            }
            
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