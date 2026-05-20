using Data;
using LeagueCoreService.ScheduledJobs;

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
                new CleanupQueueScheduledJob(queueDataService)
                    {Interval = TimeSpan.FromHours(24)},
                new SyncMatchesScheduledJob(queueDataService),
                new PostUpcomingMatchScheduledJob(queueDataService, leagueDataService),
                new PostDiscordPollScheduledJob(queueDataService)
            ];
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _logger.LogInformation("SchedulerWorker starting at: {time}", DateTimeOffset.Now);

                foreach (var job in _scheduledJobs )
                {
                    await job.Init();
                }
                
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
