namespace LeagoClient
{
    public partial class ArenasClient
    {
        static partial void UpdateJsonSerializerSettings(Newtonsoft.Json.JsonSerializerSettings settings)
        {
            settings.Converters.Add(new Shared.Converter.FallbackDateTimeOffsetConverter());
        }
    }
}