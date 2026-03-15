using NetCord.Services.ApplicationCommands;
using NetCord;
using NetCord.Rest;
using Shared.Enum;

namespace Discord;

public class BotCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    
    
    [SlashCommand("ping", "Ping command")]
    public async Task Ping()
    {
        await this.Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
        await this.FollowupAsync("ping");
    }
    
    [SlashCommand("change-rank", "Request a rank Change")]
    public async Task ChangeRank(string rank)
    {
        await this.Context.Interaction.SendResponseAsync(InteractionCallback.Message("Your rank change request has been submitted!"));
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