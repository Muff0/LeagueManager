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
        private readonly IScheduledJob[] _scheduledJobs;
    
        
        public SchedulerWorker(ILogger<SchedulerWorker> logger, 
            QueueDataService queueDataService,
            LeagueDataService leagueDataService)
        {
            _logger = logger;

            _scheduledJobs =
            [
                new SyncMatchesScheduledJob(queueDataService),
                new PostUpcomingMatchScheduledJob(queueDataService, leagueDataService)
            ];
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
                    var shouldRun = await job.ShouldRun(iterationTimestamp);
                    if (shouldRun)
                    {
                            _logger.LogInformation("Running Scheduled Job " + job.GetType());
                        
                        await job.Enqueue();   
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
