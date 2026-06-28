using Newtonsoft.Json;
using Shared.Queue;

namespace Shared.Extensions;

public static class CommandPayloadExtensions
{
    public static string SerializePayload(this CommandPayload payload)
    {
        return JsonConvert.SerializeObject(payload);
    }
}