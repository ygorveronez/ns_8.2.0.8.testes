using System;
using System.Collections;
using System.Linq;

namespace SGT.WebAdmin.Controllers
{
    public static class RequestExtension
    {
        public static string Params(this HttpRequest request, string key)
        {
            if (request.Query.ContainsKey(key))
            {
                return request.Query[key];
            }

            if (request.HasFormContentType && request.Form.ContainsKey(key))
            {
                return request.Form[key];
            }

            return null;
        }

        public static bool GetBoolParam(this HttpRequest request, string paramName, bool valorPadrao = default(bool))
        {
            return request.Params(paramName).ToBool(valorPadrao);
        }

        public static DateTime GetDateMonthParam(this HttpRequest request, string paramName, char separador, bool InicioMes, DateTime valorPadrao = default(DateTime))
        {
            if (!string.IsNullOrEmpty(request.Params(paramName)))
            {
                var data = request.Params(paramName).Split(separador);
                if (InicioMes)
                    return new DateTime(data[1].ToInt(), data[0].ToInt(), 1);
                else
                {
                    DateTime datafim = new DateTime(data[1].ToInt(), data[0].ToInt(), 05);
                    return new DateTime(data[1].ToInt(), data[0].ToInt(), datafim.LastDayOfMonth().Day);
                }
            }
            else
                return request.Params(paramName).ToDateTime(valorPadrao);
        }

        public static DateTime GetDateTimeParam(this HttpRequest request, string paramName, DateTime valorPadrao = default(DateTime))
        {
            return request.Params(paramName).ToDateTime(valorPadrao);
        }

        public static decimal GetDecimalParam(this HttpRequest request, string paramName, decimal valorPadrao = default(decimal))
        {
            return request.Params(paramName).ToDecimal(valorPadrao);
        }

        public static double GetDoubleParam(this HttpRequest request, string paramName, double valorPadrao = default(double))
        {
            return request.Params(paramName).ToDouble(valorPadrao);
        }

        public static TEnum GetEnumParam<TEnum>(this HttpRequest request, string paramName, TEnum valorPadrao = default(TEnum)) where TEnum : struct
        {
            return request.Params(paramName).ToEnum(valorPadrao);
        }

        public static int GetIntParam(this HttpRequest request, string paramName, int valorPadrao = default(int))
        {
            return request.Params(paramName).ToInt(valorPadrao);
        }

        public static System.Collections.Generic.List<TEnum> GetListEnumParam<TEnum>(this HttpRequest request, string paramName) where TEnum : struct
        {
            System.Collections.Generic.List<string> lista = request.GetListParam<string>(paramName);

            return (from valor in lista where valor.ToNullableEnum<TEnum>().HasValue select valor.ToEnum<TEnum>()).ToList();
        }

        public static System.Collections.Generic.List<T> GetListParam<T>(this HttpRequest request, string paramName)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<T>>(request.Params(paramName)) ?? new System.Collections.Generic.List<T>();
            }
            catch (Exception)
            {
                return new System.Collections.Generic.List<T>();
            }
        }

        public static T[] GetArrayParam<T>(this HttpRequest request, string paramName)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T[]>(request.Params(paramName)) ?? new T[] { };
            }
            catch (Exception)
            {
                return new T[] { };
            }
        }

        public static T[] TryGetArrayParam<T>(this HttpRequest request, string paramName)
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T[]>(request.Params(paramName)) ?? new T[] { };
            }
            catch (Exception)
            {
                string param = request.GetStringParam(paramName);
                if (!string.IsNullOrWhiteSpace(param))
                    return new T[] { (T)(object)param };
                else
                    return new T[] { };
            }
        }

        public static long GetLongParam(this HttpRequest request, string paramName, long valorPadrao = default(long))
        {
            return request.Params(paramName).ToLong(valorPadrao);
        }

        public static string GetStringParam(this HttpRequest request, string paramName, string valorPadrao = default(string))
        {
            valorPadrao = valorPadrao ?? string.Empty;

            return request.GetNullableStringParam(paramName) ?? valorPadrao;
        }

        public static TimeSpan GetTimeParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToTime();
        }

        public static bool? GetNullableBoolParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableBool();
        }

        public static DateTime? GetNullableDateTimeParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableDateTime();
        }

        public static decimal? GetNullableDecimalParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableDecimal();
        }

        public static double? GetNullableDoubleParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableDouble();
        }

        public static Nullable<TEnum> GetNullableEnumParam<TEnum>(this HttpRequest request, string paramName) where TEnum : struct
        {
            return request.Params(paramName).ToNullableEnum<TEnum>();
        }

        public static int? GetNullableIntParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableInt();
        }

        public static System.Collections.Generic.List<T> GetNullableListParam<T>(this HttpRequest request, string paramName)
        {
            try
            {
                System.Collections.Generic.List<T> lista = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<T>>(request.Params(paramName));

                if ((lista == null) || (lista.Count == 0))
                    return null;

                return lista;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static long? GetNullableLongParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableLong();
        }

        public static string GetNullableStringParam(this HttpRequest request, string paramName)
        {
            var valor = request.Params(paramName);

            return string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
        }

        public static TimeSpan? GetNullableTimeParam(this HttpRequest request, string paramName)
        {
            return request.Params(paramName).ToNullableTime();
        }

    }
}
