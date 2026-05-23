using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Shared.Enum;
using Shared.Extensions;
using Shared.Queue;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class PostDiscordPollScheduledJob : WeeklyScheduledJob<PollSchedulerService>
{
    public PostDiscordPollScheduledJob(QueueDataService queueDataService,
        PollSchedulerService pollSchedulerService)
         : base(queueDataService, pollSchedulerService)
    {
    }

    public override string Command => "PostDiscordPoll";

    public override async Task Init()
    {
        var lastRun = await _queueDataService.RunQueryAsync(new GetLastPostedPollQuery());
        if (lastRun != null)
            LastRun = lastRun.ProcessedAtUtc.GetValueOrDefault();

        await SendNextPollNotification("Poll scheduler initialized. ", DiscordNotificationType.Admin);
    }

    public async override Task OnEnqueued()
    {
        await SendNextPollNotification("A new Poll has been posted. ", DiscordNotificationType.Poll);
        await base.OnEnqueued();
    }

    private async Task SendNextPollNotification(string message, DiscordNotificationType notificationType)
    {
        await _queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
        {
            NewCommand = new CommandMessage()
            {
                Type = "SendNextPollNotification",
                Payload = new SendNextPollNotificationPayload()
                {
                    Message = message,
                    DiscordNotificationType = notificationType
                }.SerializePayload()
            }
        });
    }
}