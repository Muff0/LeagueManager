using Shared.Enum;

namespace Shared.Queue;

public class AwardApplicationPayload : CommandPayload
{
    public string GameLink { get; set; } = string.Empty;
    public AwardType AwardType { get; set; }
}