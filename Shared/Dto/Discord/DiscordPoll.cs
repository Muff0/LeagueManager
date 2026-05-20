namespace Shared.Dto.Discord;

public class DiscordPoll
{
    public string Question { get; set; } = "";

    public DiscordPollOption[] Answers { get; set; } = [];

}