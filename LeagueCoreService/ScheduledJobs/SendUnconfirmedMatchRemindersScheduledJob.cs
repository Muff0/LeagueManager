using Data;
using Shared.Services;

namespace LeagueCoreService.ScheduledJobs;

public class SendUnconfirmedMatchRemindersScheduledJob(
    QueueDataService queueDataService,
    SendUnconfirmedMatchRemindersSchedulerService schedulerService)
    : ScheduledJobBase<SendUnconfirmedMatchRemindersSchedulerService>(queueDataService, schedulerService)
{
    public override string Command => "SendUnconfirmedMatchReminders";
}