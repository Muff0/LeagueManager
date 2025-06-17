using Newtonsoft.Json;

namespace Shared.Converter
{
    public class FallbackDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        private static readonly DateTimeOffset DefaultFallback = DateTimeOffset.MinValue; // Use your preferred fallback

        public override DateTimeOffset ReadJson(JsonReader reader, Type objectType, DateTimeOffset existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value?.ToString() == "0001-01-01T00:00:00")
            {
                return DefaultFallback;
            }

            return DateTimeOffset.TryParse(reader.Value?.ToString(), out var result) ? result : DefaultFallback;
        }

        public override void WriteJson(JsonWriter writer, DateTimeOffset value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString("o")); // "o" ensures ISO 8601 format
        }
    }
}