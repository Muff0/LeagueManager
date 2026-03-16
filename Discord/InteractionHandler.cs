using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace Discord;

public class InteractionHandler(
    ApplicationCommandService<ApplicationCommandContext> commandService,
    GatewayClient client,
    IServiceProvider serviceProvider)
    : IInteractionCreateGatewayHandler
{
    public async ValueTask HandleAsync(Interaction interaction)
    {
        if (interaction is not ApplicationCommandInteraction commandInteraction)
            return;

        var result = await commandService.ExecuteAsync(
            new ApplicationCommandContext(commandInteraction, client));

        if (result is not IFailResult failResult)
            return;

        try
        {
            await interaction.SendResponseAsync(
                InteractionCallback.Message(failResult.Message));
        }
        catch { }
    }
}