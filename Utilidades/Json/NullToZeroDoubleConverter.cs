using System;
using Newtonsoft.Json;

namespace Utilidades.Json;

public class NullToZeroDoubleConverter : JsonConverter<double>
{
    public override double ReadJson(JsonReader reader, Type objectType, double existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
            return 0d;
        return Convert.ToDouble(reader.Value);
    }

    public override void WriteJson(JsonWriter writer, double value, JsonSerializer serializer)
    {
        writer.WriteValue(value);
    }
}
