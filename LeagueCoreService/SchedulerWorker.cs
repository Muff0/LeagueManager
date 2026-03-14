using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagueCoreService.ScheduledJobs;
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
                new SyncMatchesScheduledJob(),
            };
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.LogInformation("SchedulerWorker starting at: {time}", DateTimeOffset.Now);
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();


                await Task.Delay(60000, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            try
            {
                var iterationTimestamp = DateTime.Now;
                foreach (var job in _scheduledJobs)
                {
                    if (job.ShouldRun(iterationTimestamp))
                    {
                            _logger.LogInformation("Running Scheduled Job " + job.GetType());
                        
                        await job.Enqueue(_queueDataService);   
                    }

                }
            }
            catch(Exception e)
            {
                _logger.LogError(e,e.Message);
            }
        }
    }
}
