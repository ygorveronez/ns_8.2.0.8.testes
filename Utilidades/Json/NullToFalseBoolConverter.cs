using System;
using Newtonsoft.Json;

namespace Utilidades.Json;

public class NullToFalseBoolConverter : JsonConverter<bool>
{
    public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return false;
        return Convert.ToBoolean(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}
