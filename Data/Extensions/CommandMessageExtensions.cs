using Data.Model;
using Newtonsoft.Json;

namespace Data;

public static class CommandMessageExtensions
{
    public static T? GetPayload<T>(this CommandMessage cm) 
        => JsonConvert.DeserializeObject<T>(cm.Payload);
    
}