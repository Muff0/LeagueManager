using Newtonsoft.Json;
using Shared.Settings;

namespace Shared.Services;

public class TimeIntervalSchedulerService() : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastOccurrence, string? settingsJson, DateTime? currentTime = null)
    {
        var settings = JsonConvert.DeserializeObject<TimeIntervalJobSettings>(settingsJson);
        var now = currentTime ?? DateTime.UtcNow;

        var candidate = lastOccurrence.Date
            .AddSeconds(settings.IntervalSeconds);

        return candidate <= now ? now : candidate;
    }
}
