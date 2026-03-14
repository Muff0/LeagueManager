using Data;

namespace LeagueCoreService.Interfaces;

public interface IScheduledJob
{
    bool ShouldRun(DateTime now);
    Task Enqueue(QueueDataService queueDataService);
}