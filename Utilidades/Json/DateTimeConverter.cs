using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Utilidades.Json
{
    public class DateTimeConverter : JsonConverter
    {
        #region Atributos

        private readonly string _format;
        private readonly bool _writeMinValueAsEmpty;

        #endregion

        #region Construtores

        public DateTimeConverter() : this(format: "dd/MM/yyyy HH:mm:ss", writeMinValueAsEmpty: false) { }

        public DateTimeConverter(string format) : this(format, writeMinValueAsEmpty: false) { }

        public DateTimeConverter(string format, bool writeMinValueAsEmpty)
        {
            _writeMinValueAsEmpty = writeMinValueAsEmpty;
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
            return objectType == typeof(DateTime) || objectType == typeof(System.DateTime?);
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

            if (((System.DateTime)value == System.DateTime.MinValue) && _writeMinValueAsEmpty)
            {
                writer.WriteValue("");
                return;
            }

            writer.WriteValue(((System.DateTime)value).ToString(_format, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
