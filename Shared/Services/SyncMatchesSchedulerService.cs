using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Shared.Services;

public class SyncMatchesSchedulerService(IOptions<SchedulerSettings> settings) : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastPost, DateTime? currentTime = null)
    {
        var now = currentTime ?? DateTime.UtcNow;

        var candidate = lastPost.Date
            .AddMinutes(settings.Value.SyncMatchesIntervalMinutes);

        return candidate <= now ? now : candidate;
    }
}