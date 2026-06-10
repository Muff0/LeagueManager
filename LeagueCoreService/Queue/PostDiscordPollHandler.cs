using Data;
using Data.Commands.Queue;
using Data.Model;
using Data.Queries;
using Discord;
using Shared.Dto.Discord;
using Shared.Enum;

namespace LeagueCoreService.Queue;

public class PostDiscordPollHandler(
    DiscordService discordService,
    QueueDataService queueDataService)
    : ICommandHandler
{
    public string CommandType => "PostDiscordPoll";

    public async Task HandleAsync(CommandMessage cmd)
    {
        var nextPoll = await queueDataService.RunQueryAsync(
            new GetNextPollQuery());

        if (nextPoll == null)
            return;

        var payload = nextPoll.GetPayload<DiscordPoll>();

        if (payload == null)
            return;

        await discordService.PostPoll(payload);
        await queueDataService.ExecuteAsync(new SetPollStatusCommand
        {
            PollId = nextPoll.Id,
            NewStatus = QueueStatus.Completed,
            UpdateProcessedTime = true
        });
    }
}