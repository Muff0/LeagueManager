using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagueCoreService.Interfaces;
using LeagueCoreService.Jobs;
using LeagueCoreService.Services;
using Shared.Enum;

namespace LeagueCoreService
{
    public class SchedulerWorker : BackgroundService
    {
        private readonly ILogger<SchedulerWorker> _logger;
        private readonly QueueDataService _queueDataService;
        private readonly IScheduledJob[] _scheduledJobs;

        public SchedulerWorker(ILogger<SchedulerWorker> logger, IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            _queueDataService = scope.ServiceProvider.GetRequiredService<QueueDataService>();
            _logger = logger;

            _scheduledJobs = new[]
            {
                new ScheduledJobBase(),
                new ScheduledJobBase(),
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            var iterationTimestamp = DateTime.Now;
            foreach (var job in _scheduledJobs)
            {
                if (job.ShouldRun(iterationTimestamp))
                    job.Enqueue(_queueDataService);

            }
        }
    }
}
