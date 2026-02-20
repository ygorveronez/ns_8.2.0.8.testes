using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace EmissaoCTe.API
{
    public class JsonNetResult : JsonResult
    {

        #region Variaveis Globais

        public JsonSerializerSettings SerializerSettings;
        public Formatting Formatting;

        #endregion

        #region Construtores

        public JsonNetResult()
        {
            this.SerializerSettings = new JsonSerializerSettings();
            this.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());
        }

        #endregion

        #region Metodos

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentException("context");

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrWhiteSpace(ContentType) ? "application/json" : ContentType;

            if (ContentEncoding != null)
                response.ContentEncoding = ContentEncoding;

            if (Data != null)
            {
                JsonTextWriter writer = new JsonTextWriter(response.Output) { Formatting = Formatting };

                var serializer = JsonSerializer.Create(SerializerSettings);
                serializer.Serialize(writer, Data);

                writer.Flush();
            }
        }

        #endregion

    }
}