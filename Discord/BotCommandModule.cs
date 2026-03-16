using Data;
using Discord.Dto;
using NetCord.Services.ApplicationCommands;
using NetCord;
using NetCord.Rest;
using Shared.Enum;

namespace Discord;

public class BotCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private CommandOrchestrator _commandOrchestrator;

    private async Task DeferAsync()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
    }
    
    public BotCommandModule(CommandOrchestrator commandOrchestrator)
    {
        _commandOrchestrator = commandOrchestrator;
    }
    [SlashCommand("my-opponent", "Get your opponent for a specific round")]
    public async Task MyOpponent(int round)
    {
        await DeferAsync();
        var response = await _commandOrchestrator.GetOpponentAsync(
            new GetOpponentAsyncInDto()
            {
                PlayerDiscordId = Context.Interaction.User.Id,
                PlayerDiscordHandle = Context.Interaction.User.Username,
                Round = round
            });
        await FollowupAsync(response);
    }
    
    [SlashCommand("ping", "Ping command")]
    public async Task Ping()
    {
        await this.Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
        await this.FollowupAsync("ping");
    }
    
    [SlashCommand("change-rank", "Request a rank Change")]
    public static string ChangeRank(string rank)
    {
        return "Your rank change request has been submitted!";
    }
    
    [SlashCommand("report-forfeit", "Report a forfeit")]
    public async Task ReportForfeit()
    {
        await this.Context.Interaction.SendResponseAsync(InteractionCallback.Message( "Your forfeit report has been submitted! It will be reviewed by the organizers."));
    }
    
    [SlashCommand("set-vc-availability", "Set Availability for VC")]
    public async Task SetVcAvailability()
    {
        await this.Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
        await Context.Interaction.SendFollowupMessageAsync("Your request has been submitted!");
    }
    
}