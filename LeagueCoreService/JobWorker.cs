using Data;
using Data.Queries;
using LeagueCoreService.Extensions;
using LeagueCoreService.ScheduledJobs;

namespace LeagueCoreService
{
    public class JobWorker(
        ILogger<JobWorker> logger,
        IServiceScopeFactory scopeFactory,
        QueueDataService queueDataService,
        IJobRegistryCache cache) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            await SeedRegistryAsync();

            while (!ct.IsCancellationRequested)
            {
                await TickAsync(ct);
                await Task.Delay(TimeSpan.FromSeconds(30), ct);
            }
        }
        private async Task TickAsync(CancellationToken ct)
        {
            using var scope = scopeFactory.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<IScheduledJob>();

            foreach (var job in jobs)
            {
                var settings = cache.GetSettingsJson(job.JobType);
                if (job.IsDue(DateTime.Now, cache.GetLastRunAt(job.JobType), settings))
                    await RunAsync(job, settings, ct);
            }
        }
        
        private async Task RunAsync(IScheduledJob job, string? settings, CancellationToken ct)
        {
            if (!cache.IsEnabled(job.JobType))
            {
                logger.LogDebug("Job {JobType} is disabled, skipping", job.JobType);
                return;
            }

            logger.LogInformation("Running job {JobType}", job.JobType);

            try
            {
                await job.ExecuteAsync(settings, ct);
                cache.SetLastRunAt(job.JobType, DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Job {JobType} failed", job.JobType);
                // still set LastRunAt so a failing job doesn't hammer on every tick
                cache.SetLastRunAt(job.JobType, DateTime.UtcNow);
            }
        }

        private async Task SeedRegistryAsync()
        {
            using var scope = scopeFactory.CreateScope();
            var jobs = scope.ServiceProvider.GetServices<IScheduledJob>().ToList();
            var registeredTypes = jobs.Select(j => j.JobType).ToHashSet();

            var existing = await queueDataService.RunQueryAsync(new GetJobRegistryQuery());
            var existingTypes = existing.Select(r => r.JobType).ToHashSet();

            foreach (var job in jobs.Where(j => !existingTypes.Contains(j.JobType)))
                await queueDataService.CreateJobRegistryEntryAsync(job.JobType, job.DefaultSettingsJson);

            var all = await queueDataService.RunQueryAsync(new GetJobRegistryQuery());
            cache.Reload(all.AsReadOnly(), registeredTypes);

            logger.LogInformation("Job registry seeded. Registered: {Types}",
                string.Join(", ", registeredTypes));
        }
    }
}