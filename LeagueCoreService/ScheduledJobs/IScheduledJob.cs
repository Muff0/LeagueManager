using Data;

namespace LeagueCoreService.ScheduledJobs;

public interface IScheduledJob
{
    bool ShouldRun(DateTime now);
    Task Enqueue(QueueDataService queueDataService);
}