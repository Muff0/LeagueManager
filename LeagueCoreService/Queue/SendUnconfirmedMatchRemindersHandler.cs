using Data;
using Data.Commands.Queue;
using Data.Extensions;
using Data.Model;
using Data.Queries;
using Mail.MessageBuilders;
using Shared.Extensions;
using Shared.Queue;

namespace LeagueCoreService.Queue;

public class SendUnconfirmedMatchRemindersHandler(
    QueueDataService queueDataService,
    LeagueDataService leagueDataService) : ICommandHandler
{
    public string CommandType => "SendUnconfirmedMatchReminders";

    public async Task HandleAsync(CommandMessage cmd)
    {
        var season = (await leagueDataService.TakeFirstAsync(new GetActiveSeasonQuery()))!
            .ToSeasonDto();
        var unconfirmedMatches = (await leagueDataService.RunQueryAsync(
            new GetUnconfirmedMatchesQuery
            {
                IncludePlayers = true,
                SeasonId = season.Id
            })).Select(m => m.ToMatchDto()).ToArray();
        // Hacky but should be ok for now
        var currentRound = unconfirmedMatches.Max(um => um.Round);

        foreach (var currentMatch in unconfirmedMatches)
        {
            if (currentMatch.Round != currentRound)
                continue;
            var msg = new MatchReminderMessage(currentMatch, NextMondayNoon(), season);
            var p1 = currentMatch.Players![0].Player!;
            var p2 = currentMatch.Players![1].Player!;

            var payload = new SendEmailPayload
            {
                HtmlBody = msg.HtmlBody,
                Subject = msg.Subject,
                Tos = [p1.EmailAddress],
                Ccs = [p2.EmailAddress]
            };
            await queueDataService.ExecuteAsync(
                new InsertCommandMessageCommand
                {
                    NewCommand = new CommandMessage
                    {
                        Type = "SendEmail",
                        Payload = payload.SerializePayload()
                    }
                });
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