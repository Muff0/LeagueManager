using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Discord;
using Newtonsoft.Json;
using Shared.Dto.Discord;
using Shared.Enum;
using Shared.Extensions;
using Shared.Queue;
using Shared.Services;
using Shared.Settings;

namespace LeagueCoreService.ScheduledJobs;

public class PostDiscordPollScheduledJob(QueueDataService queueDataService, 
    DiscordService discordService,
    DayOfWeekSchedulerService schedulerService)
    : ScheduledJobBase<DayOfWeekSchedulerService>(schedulerService)
{
    public override string JobType => "PostDiscordPoll";

    
    public override async Task ExecuteAsync(string? settingsJson, CancellationToken ct)
    {
        var nextPoll = await queueDataService.TakeFirstAsync(
            new GetNextPollQuery());

        if (nextPoll == null)
            return;

        var payload = nextPoll.GetPayload<DiscordPoll>();

        if (payload == null)
            return;

        await discordService.PostPoll(payload);
        await queueDataService.ExecuteAsync(new SetPollStatusCommand
        {
            PollId = nextPoll.Id,
            NewStatus = QueueStatus.Completed,
            UpdateProcessedTime = true
        });
        
        await SendNextPollNotification("A new Poll has been posted. ", DiscordNotificationType.Poll,settingsJson);
    }

    public override string? DefaultSettingsJson => JsonConvert.SerializeObject(new DayOfWeekJobSettings()
    {
        DayOfWeek = DayOfWeek.Friday,
        HourOfDayUtc = 16
    });
    private async Task SendNextPollNotification(string message, DiscordNotificationType notificationType, string? settingsJson)
    {
        var lastRun = await queueDataService.TakeFirstAsync(new GetLastPostedPollQuery());
        var nextRun = schedulerService.GetNextOccurrence(lastRun.ProcessedAtUtc.GetValueOrDefault(),settingsJson);
        var pollQueue = await queueDataService.CountAsync(new GetPollsInQueueQuery());

        await discordService.SendPollNotification(pollQueue, nextRun, message, notificationType);
    }
}