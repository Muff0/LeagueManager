using Newtonsoft.Json;
using Shared.Settings;

namespace Shared.Services;

public class TimeIntervalSchedulerService() : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastOccurrenceUtc, string? settingsJson, DateTime? currentTimeUtc = null)
    {
        var settings = JsonConvert.DeserializeObject<TimeIntervalJobSettings>(settingsJson);
        var now = currentTimeUtc ?? DateTime.UtcNow;

        var candidate = lastOccurrenceUtc
            .AddSeconds(settings.IntervalSeconds);

        return candidate <= now ? now : candidate;
    }
}
