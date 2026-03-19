namespace Shared.Queue;

public class ReportForfeitPayload : ICommandPayload
{
    public ulong DiscordId { get; set; }
    public int Round { get; set; }
    public string Comments { get; set; } = string.Empty;
}