using Microsoft.Extensions.Options;
using Shared.Settings;

namespace Shared.Services;

public class SendGameAnalysisSchedulerService(IOptions<SchedulerSettings> settings) : IJobSchedulerService
{
    public DateTime GetNextOccurrence(DateTime lastPost, DateTime? currentTime = null)
    {
        var s = settings.Value;
        var now = currentTime ?? DateTime.UtcNow;

        var candidate = lastPost.Date
            .AddMinutes(s.QueueGameAnalysisIntervalMinutes);

        return candidate <= now ? now : candidate;
    }
}