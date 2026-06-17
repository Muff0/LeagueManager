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
                SettingsUpdatedAtUtc = DateTime.UtcNow
            }));

    }

    public static async Task UpdateJobLastRun(this QueueDataService queueDataService, string jobType)
    {
        await queueDataService.ExecuteAsync(
            new UpdateJobLastRunCommand()
            {
                JobType = jobType,
                LastRun = DateTime.UtcNow
            });
    }
}