using System.Security.Principal;
using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagueCoreService.Services;
using Shared.Enum;

namespace LeagueCoreService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly MainService _mainService;
        private readonly QueueDataService _queueDataService;

        public Worker(ILogger<Worker> logger, IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            _mainService = scope.ServiceProvider.GetRequiredService<MainService>();
            _queueDataService = scope.ServiceProvider.GetRequiredService<QueueDataService>();
            _logger = logger;
        }

        public async Task<CommandMessage> GetNextCommand()
        {
            return await _queueDataService.RunQueryAsync(
                new GetNextCommandMessageQuery()
                {

                });
        }

        public async Task SetCommandStatus(CommandMessage cmd, QueueStatus newStatus, bool updateMetadata = true)
        {
            await _queueDataService.ExecuteAsync(
                new SetCommandMessageStatusCommand()
                {
                    CommandMessageId = cmd.Id,
                    NewStatus = newStatus,
                    UpdateProcessedTime = updateMetadata,
                    RaiseRetriesCounter = updateMetadata
                });
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
            var cmd = await GetNextCommand();

            if (cmd == null)
                return;
            try
            {

                await SetCommandStatus(cmd, QueueStatus.Processing, false);

                if (cmd.Type == "SendNotification")
                {

                    await _mainService.SendNotification();
                    await SetCommandStatus(cmd, QueueStatus.Completed);

                }
                else
                {
                    await SetCommandStatus(cmd, QueueStatus.Failed);
                }
            }
            catch (Exception ex)
            {
                await SetCommandStatus(cmd, QueueStatus.Failed);
            }
        }
    }
}
