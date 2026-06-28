namespace Shared.Queue;

public class RankChangePayload : CommandPayload
{
    public ulong DiscordId { get; set; }
    public string NewRank { get; set; } = string.Empty;
}