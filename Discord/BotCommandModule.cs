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
        this.Context.Interaction.SendFollowupMessageAsync("ping");
    }
    
    // [SlashCommand("ChangeRank", "Request a rank Change")]
    // public async Task ChangeRank(ApplicationCommandContext ctx, PlayerRank rank, [SlashCommandParameter]User? user = null)
    // {
    //     await ctx.Interaction.SendFollowupMessageAsync("Your rank change request has been submitted!");
    // }
    //
    // [SlashCommand("ReportForfeit", "Report an opponent not showing up")]
    // public async Task ReportForfeit(ApplicationCommandContext ctx, [SlashCommandParameter]User? user = null)
    // {
    //     await ctx.Interaction.SendFollowupMessageAsync( "Your forfeit report has been submitted! It will be reviewed by the organizers.");
    // }
    //
    // [SlashCommand("SetVCAvailability", "Set Availability for VC")]
    // public async Task SetVcAvailability(ApplicationCommandContext ctx)
    // {
    //     await ctx.Interaction.SendFollowupMessageAsync("Your request has been submitted!");
    // }
    
}