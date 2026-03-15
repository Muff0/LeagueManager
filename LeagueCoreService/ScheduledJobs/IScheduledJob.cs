using Data;

namespace LeagueCoreService.ScheduledJobs;

public interface IScheduledJob
{
    Task<bool> ShouldRun(DateTime now);
    Task Enqueue();
}