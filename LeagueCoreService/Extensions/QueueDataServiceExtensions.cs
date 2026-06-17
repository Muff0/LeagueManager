using Data;
using Data.Commands;
using Data.Commands.Queue;
using Data.Model;

namespace LeagueCoreService.Extensions;

public static class QueueDataServiceExtensions
{
    public static async Task CreateJobRegistryEntryAsync(this QueueDataService queueDataService, string jobType,
        string? defaultSettingsJson)
    {
        await queueDataService.ExecuteAsync(new InsertEntityCommand<QueueContext>(
            new JobRegistry()
            {
                IsEnabled = true,
                JobType = jobType,
                SettingsJson = defaultSettingsJson,
                SettingsUpdatedAt = DateTime.Now
            }));

    }
}