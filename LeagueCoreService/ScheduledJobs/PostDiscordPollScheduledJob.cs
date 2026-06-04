using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Shared.Enum;
using Shared.Extensions;
using Shared.Queue;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class PostDiscordPollScheduledJob(QueueDataService queueDataService, PollSchedulerService schedulerService) : ScheduledJobBase<PollSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "PostDiscordPoll";


    public async override Task OnEnqueued()
    {
        await SendNextPollNotification("A new Poll has been posted. ", DiscordNotificationType.Poll);
        await base.OnEnqueued();
    }

    private async Task SendNextPollNotification(string message, DiscordNotificationType notificationType)
    {
        await queueDataService.ExecuteAsync(new InsertCommandMessageCommand()
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