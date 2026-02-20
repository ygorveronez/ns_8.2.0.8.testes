using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Utilidades.Json
{
    public class DecimalConverter : JsonConverter
    {
        #region Atributos

        private readonly string _format;

        #endregion

        #region Construtores

        public DecimalConverter() : this(format: "F") { }

        public DecimalConverter(string format)
        {
            _format = format;
        }

        #endregion

        #region Métodos Público

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((decimal)value).ToString(_format, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
