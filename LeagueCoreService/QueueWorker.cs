using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using LeagueCoreService.Queue;
using LeagueCoreService.Services;
using Shared.Enum;

namespace LeagueCoreService
{
    public class QueueWorker : BackgroundService
    {
        private readonly ILogger<QueueWorker> _logger;
        private readonly MainService _mainService;
        private readonly QueueDataService _queueDataService;
        private readonly Dictionary<string, ICommandHandler> _handlers;
        
        public QueueWorker(ILogger<QueueWorker> logger, IServiceScopeFactory scopeFactory)
        {
            using var scope = scopeFactory.CreateScope();
            _mainService = scope.ServiceProvider.GetRequiredService<MainService>();
            _queueDataService = scope.ServiceProvider.GetRequiredService<QueueDataService>();
            _logger = logger;
            _handlers = scope.ServiceProvider
                .GetServices<ICommandHandler>()
                .ToDictionary(h => h.CommandType);
        }

        private async Task<CommandMessage?> GetNextCommand()
        {
            return await _queueDataService.RunQueryAsync(
                new GetNextCommandMessageQuery()
                {
                });
        }

        private async Task SetCommandStatus(CommandMessage cmd, QueueStatus newStatus, bool updateMetadata = true)
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
            
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("QueueWorker starting at: {time}", DateTimeOffset.Now);
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWork();

                await Task.Delay(10000, stoppingToken);
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

                if (!_handlers.TryGetValue(cmd.Type, out var handler))
                {
                    _logger.LogWarning("No handler found for command type: {type}", cmd.Type);
                    await SetCommandStatus(cmd, QueueStatus.Failed);
                    return;
                }

                await handler.HandleAsync(cmd);
                await SetCommandStatus(cmd, QueueStatus.Completed);
            }
            catch (Exception ex)
            {
                await SetCommandStatus(cmd, QueueStatus.Failed);
            }
        }
    }
}