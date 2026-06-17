using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Shared.Settings;

namespace Shared.Services;

public class DayOfWeekSchedulerService : IJobSchedulerService
{

    public DateTime GetNextOccurrence(DateTime lastOccurrence,string? jsonSettings, DateTime? currentTime = null)
    {
        var settings = JsonConvert.DeserializeObject<DayOfWeekJobSettings>(jsonSettings);
        var now = currentTime ?? DateTime.UtcNow;

        var daysUntil = ((int)settings.DayOfWeek - (int)lastOccurrence.DayOfWeek + 7) % 7;
        if (daysUntil == 0) daysUntil = 7;

        var candidate = lastOccurrence.Date
            .AddDays(daysUntil)
            .AddHours(settings.HourOfDayUtc);

        return candidate <= now ? now : candidate;
    }
}