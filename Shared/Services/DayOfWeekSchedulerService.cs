using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Settings;

namespace Shared.Services;

public class DayOfWeekSchedulerService : IJobSchedulerService
{

    public DateTime GetNextOccurrence(DateTime lastOccurrenceUtc,string? jsonSettings, DateTime? currentTimeUtc = null)
    {
        var settings = JsonConvert.DeserializeObject<DayOfWeekJobSettings>(jsonSettings);
        var now = currentTimeUtc ?? DateTime.UtcNow;

        var daysUntil = ((int)settings.DayOfWeek - (int)lastOccurrenceUtc.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;

        var candidate = lastOccurrenceUtc.Date
            .AddDays(daysUntil)
            .AddHours(settings.HourOfDayUtc);

        return candidate <= now ? now : candidate;
    }
}