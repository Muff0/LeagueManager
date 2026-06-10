using Newtonsoft.Json;
using Shared.Queue;

namespace Shared.Extensions;

public static class ICommandPayloadExtensions
{
    public static string SerializePayload(this ICommandPayload payload)
    {
        return JsonConvert.SerializeObject(payload);
    }
}