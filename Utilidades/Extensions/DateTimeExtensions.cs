using System.Collections.Generic;

namespace System
{
    public static class DateTimeExtensions
    {
        public static DateTime CeilingHours(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

            return (dt.Minute > 0) ? ndt.AddHours(1) : ndt;
        }

        public static DateTime CeilingMinute(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

            return (dt.Second > 0) ? ndt.AddMinutes(1) : ndt;
        }

        public static DateTime CeilingSecond(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);

            return (dt.Millisecond > 0) ? ndt.AddSeconds(1) : ndt;
        }

        public static List<DateTime> DatesBetweenWithoutWeekend(this DateTime fromDate, DateTime toDate)
        {
            List<DateTime> datesToreturn = new List<DateTime>();
            DateTime currentDate = fromDate.Date;
            DateTime deadline = toDate.Date;

            while (currentDate <= deadline)
            {
                if ((currentDate.DayOfWeek != DayOfWeek.Saturday) && (currentDate.DayOfWeek != DayOfWeek.Sunday))
                    datesToreturn.Add(currentDate);

                currentDate = currentDate.AddDays(1);
            }

            return datesToreturn;
        }

        public static double DifferenceOfHoursBetweenWorkDays(this DateTime dt, DateTime lastDate)
        {
            DateTime novaDatadt = dt;
            DateTime novaDataLastDate = lastDate;

            while (dt.Date <= lastDate.Date)
            {
                if (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                    novaDataLastDate = novaDataLastDate.AddDays(-1);

                dt = dt.AddDays(1);
            }

            return (novaDataLastDate - novaDatadt).TotalHours;
        }

        public static int DifferenceOfMonthsBetween(this DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
                return DifferenceOfMonthsBetween(toDate, fromDate);

            int monthsByYear = (toDate.Year - fromDate.Year) * 12;
            int months = (monthsByYear + toDate.Month) - fromDate.Month;

            return months;
        }

        public static int DifferenceOfDaysBetween(this DateTime fromDate, DateTime toDate)
        {
            return (int)(toDate - fromDate).TotalDays;
        }

        public static DateTime FirstDayOfMonth(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, 1);

        public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1);

        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

            if (diff < 0)
                diff += 7;

            return dt.AddDays(-diff).Date;
        }

        public static DateTime FloorHour(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

        public static DateTime FloorMinute(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

        public static DateTime FloorSecond(this DateTime dt) =>
            new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);

        public static bool IsDateSameMonth(this DateTime startDate, DateTime lastDate) =>
            startDate.Month == lastDate.Month && startDate.Year == lastDate.Year;

        public static bool IsFirstDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().Day == dt.Day;

        public static bool IsLastDayOfMonth(this DateTime dt) =>
            dt.LastDayOfMonth().Day == dt.Day;

        public static bool IsNullOrMinValue(this DateTime? dt)
        {
            return (dt == null || !dt.HasValue || dt.Value == DateTime.MinValue);
        }

        public static DateTime LastDayOfMonth(this DateTime dt) =>
            dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

        public static DateTime LastDayOfWeek(this DateTime dt) =>
            dt.FirstDayOfWeek().AddDays(6);

        public static DateTime? Min(DateTime? dt1, DateTime? dt2)
        {
            if (dt1.HasValue && dt2.HasValue)
                return dt1.Value < dt2.Value ? dt1 : dt2;

            return dt1 ?? dt2;
        }

        public static DateTime RoundHour(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, 0, 0);

            return (dt.Minute >= 30) ? ndt.AddHours(1) : ndt;
        }

        public static DateTime RoundMinute(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

            return (dt.Second >= 30) ? ndt.AddMinutes(1) : ndt;
        }

        public static DateTime RoundSecond(this DateTime dt)
        {
            DateTime ndt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);

            return (dt.Millisecond >= 500) ? ndt.AddSeconds(1) : ndt;
        }

        public static string ToDateString(this DateTime dt) =>
            dt.ToString("dd/MM/yyyy");

        public static string ToDateString(this DateTime? dt)
        {
            if (!dt.HasValue)
                return string.Empty;

            return dt.Value.ToString("dd/MM/yyyy");
        }

        public static string ToDateTimeString(this DateTime dt, bool showSeconds = false) =>
            dt.ToString($"dd/MM/yyyy HH:mm{(showSeconds ? ":ss" : "")}");

        public static string ToDateTimeString(this DateTime? dt, bool showSeconds = false)
        {
            if (!dt.HasValue)
                return string.Empty;

            return dt.Value.ToString($"dd/MM/yyyy HH:mm{(showSeconds ? ":ss" : "")}");
        }

        public static string ToDateTimeStringISO8601(this DateTime dt) =>
            dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

        public static string ToTimeString(this DateTime dt, bool showSeconds = false) =>
            dt.ToString($"HH:mm{(showSeconds ? ":ss" : "")}");

        public static long ToUnixSeconds(this DateTime datetime)
        {
            return (long)datetime.Subtract(new System.DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long ToUnixMillseconds(this DateTime datetime)
        {
            return (long)datetime.Subtract(new System.DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static string ToHourMinuteString(this DateTime? dt)
        {
            if (dt == null || !dt.HasValue)
                return string.Empty;

            return dt.Value.ToString("HH:mm");
        }

        public static bool IsBewteenTwoDates(this DateTime dt, DateTime start, DateTime end)
        {
            return dt >= start && dt <= end;
        }
    }
}
