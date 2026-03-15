using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;
using NetCord.Logging;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using Shared.Settings;

namespace Discord;

public class BotService :BackgroundService
{

    public BotService(IOptions<DiscordSettings> settings)
    {
        _settings = settings;
        var to = new BotToken(_settings.Value.Token);
        _gatewayClient = new GatewayClient(to,
            new GatewayClientConfiguration()
            {
                Intents = default,
                Logger = new ConsoleLogger()
            });
    }
    private readonly GatewayClient _gatewayClient;
    private readonly IOptions<DiscordSettings> _settings;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        var interactionService = new ApplicationCommandService<ApplicationCommandContext>();
        interactionService.AddModules(typeof(DiscordService).Assembly);
            
        var manager = new ApplicationCommandServiceManager();
        manager.AddService(interactionService);
        
        _gatewayClient.InteractionCreate += async interaction =>
        {
            if (interaction is not ApplicationCommandInteraction commandInteraction)
                return;
            
            var result =
                await interactionService.ExecuteAsync(
                    new ApplicationCommandContext((ApplicationCommandInteraction)interaction, _gatewayClient));

            if (result is not IFailResult failResult)
                return;

            try
            {
                await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
            }
            catch
            {
            }
        };
        
        // Once ready, register commands
        _gatewayClient.Ready += async args =>
        {
            var commands = await _gatewayClient.Rest.GetGlobalApplicationCommandsAsync(_settings.Value.AppId, cancellationToken:stoppingToken);

            foreach (var command in commands)
            {
                await _gatewayClient.Rest.DeleteGlobalApplicationCommandAsync(
                    _settings.Value.AppId,
                    command.Id,
                    cancellationToken: stoppingToken);
            }
            
            
            var guildCommands = await _gatewayClient.Rest.GetGuildApplicationCommandsAsync(_settings.Value.AppId, _settings.Value.ServerId, cancellationToken:stoppingToken);

            foreach (var command in guildCommands)
            {
                await _gatewayClient.Rest.DeleteGuildApplicationCommandAsync(
                    _settings.Value.AppId,
                    command.Id,
                    _settings.Value.ServerId,
                    cancellationToken: stoppingToken);
            }

            
            await manager.RegisterCommandsAsync(_gatewayClient.Rest, 
                _settings.Value.AppId,
                _settings.Value.ServerId,
                cancellationToken: stoppingToken);
        };
            
        // Start the client
        await _gatewayClient.StartAsync(null, stoppingToken);
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}