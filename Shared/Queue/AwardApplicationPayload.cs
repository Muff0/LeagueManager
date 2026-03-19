using Shared.Enum;

namespace Shared.Queue;

public class AwardApplicationPayload : ICommandPayload
{
    public string GameLink { get; set; } = string.Empty;
    public AwardType AwardType { get; set; }
       
}