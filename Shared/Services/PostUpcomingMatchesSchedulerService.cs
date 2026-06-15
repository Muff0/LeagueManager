using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Shared.Services;

public class PostUpcomingMatchesSchedulerService(IOptions<SchedulerSettings> settings) : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastPost, DateTime? currentTime = null)
    {
        var now = currentTime ?? DateTime.UtcNow;
        var candidate = lastPost.Date
            .AddMinutes(settings.Value.SendUpcomingMatchesIntervalMinutes);

        return candidate <= now ? now : candidate;
    }
}