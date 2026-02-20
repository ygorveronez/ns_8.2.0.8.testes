using System;
using System.Collections.Generic;

namespace Utilidades
{
    public class DateTime
    {
        public static System.DateTime? FromUnixSeconds(long unixSeconds)
        {
            try
            {
                if (unixSeconds == 0)
                    return null;

                if (unixSeconds > 9999999999)
                {
                    // Se o valor for maior isso, provavelmente foi mandado em milisegundos. Vamos converter só para evitar erros.
                    System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime = dateTime.AddMilliseconds(unixSeconds).ToLocalTime();
                    return dateTime;
                }
                else
                {
                    // Foi enviado em segundos corretamenteo
                    System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    dateTime = dateTime.AddSeconds(unixSeconds).ToLocalTime();
                    return dateTime;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static System.DateTime ConverterDataExcelToDateTime(double dataFormatoExcel, string dataEmString = null)
        {
            try
            {
                System.DateTime data = System.DateTime.FromOADate(dataFormatoExcel);
                return data;
            }
            catch
            {
                if (dataFormatoExcel.ToString().Length == 12)
                    return dataFormatoExcel.ToString().ToDateTime("yyyyMMddHHmm");

                return dataEmString == null ? System.DateTime.Now : dataEmString.ToDateTime();
            }
        }

        public static System.DateTime? ConverterDataExcelToNullableDateTime(double dataFormatoExcel, string dataEmString = null)
        {
            try
            {
                System.DateTime data = System.DateTime.FromOADate(dataFormatoExcel);
                return data;
            }
            catch
            {
                if (dataFormatoExcel.ToString().Length == 12)
                    return dataFormatoExcel.ToString().ToDateTime("yyyyMMddHHmm");

                return dataEmString == null ? null : dataEmString.ToNullableDateTime();
            }
        }

        public static List<System.DateTime> GetDatesBetween(System.DateTime startDate, System.DateTime endDate)
        {
            List<System.DateTime> allDates = new List<System.DateTime>();

            for (System.DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                allDates.Add(date);

            return allDates;
        }
    }
}
