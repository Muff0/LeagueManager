using Newtonsoft.Json;
using Shared.Converter;

namespace LeagoClient;

public partial class ArenasClient
{
    static partial void UpdateJsonSerializerSettings(JsonSerializerSettings settings)
    {
        settings.Converters.Add(new FallbackDateTimeOffsetConverter());
    }
}