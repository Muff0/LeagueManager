namespace Shared.Queue;

public class RankChangePayload : ICommandPayload
{
    public ulong DiscordId { get; set; }
    public string NewRank { get; set; } = string.Empty;
}