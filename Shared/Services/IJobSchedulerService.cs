namespace Shared.Services;

public interface IJobSchedulerService
{
    DateTime GetNextOccurrence(DateTime lastOccurrence, string? settingsJson, DateTime? currentTime = null);
}