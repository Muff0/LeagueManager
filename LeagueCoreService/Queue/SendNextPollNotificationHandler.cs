using Data;
using Data.Model;
using Data.Queries;
using Discord;
using Shared.Queue;
using Shared.Services;

namespace LeagueCoreService.Queue;

public class SendNextPollNotificationHandler(DiscordService discordService, 
    PollSchedulerService pollSchedulerService,
    QueueDataService queueDataService) : ICommandHandler
{
    public string CommandType => "SendNextPollNotification";

    public async Task HandleAsync(CommandMessage cmd)
    {
        var payload = cmd.GetPayload<SendNextPollNotificationPayload>();
        var lastRun = await queueDataService.RunQueryAsync(new GetLastPostedPollQuery());
        var nextRun = pollSchedulerService.GetNextOccurrence(lastRun.ProcessedAtUtc.GetValueOrDefault());
        
        var pollQueue = await queueDataService.CountAsync(new GetPollsInQueueQuery());

        await discordService.SendPollNotification(pollQueue, nextRun, payload.Message, payload.DiscordNotificationType );
    }
    
}