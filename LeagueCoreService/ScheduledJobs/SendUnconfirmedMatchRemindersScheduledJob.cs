using Data;
using Data.Commands.Queue;
using Data.Extensions;
using Data.Model;
using Data.Queries;
using Mail.MessageBuilders;
using Newtonsoft.Json;
using Shared.Extensions;
using Shared.Queue;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class SendUnconfirmedMatchRemindersScheduledJob(
    QueueDataService queueDataService,
    LeagueDataService leagueDataService,
    DayOfWeekSchedulerService schedulerService)
    : ScheduledJobBase<DayOfWeekSchedulerService>(schedulerService)
{
    public override string JobType => "SendUnconfirmedMatchReminders";
    
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
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

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(new DayOfWeekJobSettings()
    {
        DayOfWeek = DayOfWeek.Thursday,
        HourOfDayUtc = 15
    });
    public DateTime NextMondayNoon()
    {
        var today = DateTime.UtcNow.Date;
        var daysUntilMonday = ((int)DayOfWeek.Monday - (int)today.DayOfWeek + 7) % 7;
        if (daysUntilMonday == 0) daysUntilMonday = 7; // today is Monday → next Monday
        return today.AddDays(daysUntilMonday).AddHours(12);
    }
}