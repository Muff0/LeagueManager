namespace Shared.Queue;

public class ReportForfeitPayload : CommandPayload
{
    public ulong DiscordId { get; set; }
    public int Round { get; set; }
    public string Comments { get; set; } = string.Empty;
}