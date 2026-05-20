using Data.Model;
using Newtonsoft.Json;

namespace Data;

public static class PollExtension
{
    
    public static T? GetPayload<T>(this Poll cm) 
        => JsonConvert.DeserializeObject<T>(cm.Payload);

}