namespace Shared.Services;

public interface IJobSchedulerService
{
    DateTime GetNextOccurrence(DateTime lastOccurrence, DateTime? currentTime = null);
}