using Data;
using LeagueCoreService.Services;

namespace LeagueCoreService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MainService _mainService;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            _mainService = scope.ServiceProvider.GetRequiredService<MainService>();
            _logger = logger;
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
                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            var cmd = await _mainService.GetNextCommand();

            if (cmd == null)
                return;

            if (cmd.Type == "SendNotification")
                await _mainService.SendNotification();
        }
    }
}
