using System.Reflection;

namespace Utilidades.Extensions
{
    public static class ObjectExtesions
    {
        #region Métodos Públicos

        public static PropertyInfo[] ObterPropriedades<T>(this T tipo)
        {
            return tipo.GetType().GetProperties();
        }


        public static string ToJson<T>(this T data)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data);
        }

        public static string ToJson<T>(this T data, Newtonsoft.Json.JsonSerializerSettings jsonSerializerSettings, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.None)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(data, formatting, jsonSerializerSettings);
        }

        public static T FromJson<T>(this string json)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }

        #endregion
    }
}
