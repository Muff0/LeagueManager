namespace Shared.Services;

public interface IJobSchedulerService
{
    DateTime GetNextOccurrence(DateTime lastOccurrenceUtc, string? settingsJson, DateTime? currentTimeUtc = null);
}