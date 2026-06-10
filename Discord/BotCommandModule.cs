using Discord.Dto;
using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;
using Shared.Enum;
using Shared.Queue;

namespace Discord;

public class BotCommandModule : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly CommandOrchestrator _commandOrchestrator;

    public BotCommandModule(CommandOrchestrator commandOrchestrator)
    {
        _commandOrchestrator = commandOrchestrator;
    }

    private async Task DeferAsync()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage());
    }

    [SlashCommand("my-opponent", "Get your opponent for a specific round")]
    public async Task MyOpponent(int round)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        var response = await _commandOrchestrator.GetOpponentAsync(
            new GetOpponentAsyncInDto
            {
                PlayerDiscordId = Context.Interaction.User.Id,
                PlayerDiscordHandle = Context.Interaction.User.Username,
                Round = round
            });
        await FollowupAsync(new InteractionMessageProperties()
            .WithContent(response)
            .WithFlags(MessageFlags.Ephemeral));
    }

    [SlashCommand("apply-for-award", "Apply for an award")]
    public async Task ApplyForAward(string gameLink, AwardType awardType)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        _commandOrchestrator.QueueApplyForAwardCommand(
            new AwardApplicationPayload
            {
                AwardType = awardType,
                GameLink = gameLink
            });

        await FollowupAsync(new InteractionMessageProperties()
            .WithContent("Your application has been submitted!")
            .WithFlags(MessageFlags.Ephemeral));
    }

    [SlashCommand("change-rank", "Request a rank Change")]
    public async Task ChangeRank(string rank)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        _commandOrchestrator.QueueRankChangeCommand(
            new RankChangePayload
            {
                DiscordId = Context.Interaction.User.Id,
                NewRank = rank
            });

        await FollowupAsync(new InteractionMessageProperties()
            .WithContent("Your request has been submitted!")
            .WithFlags(MessageFlags.Ephemeral));
    }

    [SlashCommand("report-forfeit", "Report a forfeit")]
    public async Task ReportForfeit(int round, string comments)
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));

        _commandOrchestrator.QueueReportForfeitCommand(
            new ReportForfeitPayload
            {
                DiscordId = Context.Interaction.User.Id,
                Round = round,
                Comments = comments
            });
        await FollowupAsync(new InteractionMessageProperties()
            .WithContent("Your report has been submitted!")
            .WithFlags(MessageFlags.Ephemeral));
    }

    [SlashCommand("set-vc-availability", "Set Availability for VC")]
    public async Task SetVcAvailability()
    {
        await Context.Interaction.SendResponseAsync(InteractionCallback.DeferredMessage(MessageFlags.Ephemeral));
        await Context.Interaction.SendFollowupMessageAsync("Your request has been submitted!");
    }
}