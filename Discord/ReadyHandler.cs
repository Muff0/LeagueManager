using Microsoft.Extensions.Options;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Services.ApplicationCommands;
using Shared.Settings;

namespace Discord;

public class ReadyHandler(
    ApplicationCommandServiceManager manager,
    GatewayClient client,
    IOptions<DiscordSettings> settings)
    : IReadyGatewayHandler
{
    public async ValueTask HandleAsync(ReadyEventArgs args)
    {
        await manager.RegisterCommandsAsync(
            client.Rest,
            settings.Value.AppId,
            settings.Value.ServerId);
    }
}