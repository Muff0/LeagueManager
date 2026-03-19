using Newtonsoft.Json;
using Shared.Queue;

namespace Shared.Extensions;

public static class ICommandPayloadExtensions
{
    
    public static string SerializePayload(this ICommandPayload payload) 
        => JsonConvert.SerializeObject(payload);
}