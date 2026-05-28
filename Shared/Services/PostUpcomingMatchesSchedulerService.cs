using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Shared.Services;

public class PostUpcomingMatchesSchedulerService(IOptions<SchedulerSettings> settings) : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastPost, DateTime? currentTime = null)
    {
        var s = settings.Value;
        DateTime now = currentTime ?? DateTime.UtcNow;

        var daysUntil = ((int)s.PollPostDay - (int)lastPost.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;

        var candidate = lastPost.Date
            .AddMinutes(settings.Value.SendUpcomingMatchesIntervalMinutes);

        return candidate <= now ? now : candidate;
    }
}