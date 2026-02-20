namespace System
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan RoundUpMinute(this TimeSpan time)
        {
            TimeSpan novoTempo = new TimeSpan(time.Hours, time.Minutes, 0);

            return time.Seconds > 30 ? TimeSpan.FromMinutes(novoTempo.TotalMinutes + 1) : time;
        }

        public static string ToTimeString(this TimeSpan time, bool showSeconds = false)
        {
            return $"{((int)time.TotalHours).ToString("00"):00}:{time.Minutes.ToString("00")}{(showSeconds ? $":{time.Seconds:00}" : "")}";
        }

        public static DateTime ToDateTime(this TimeSpan time, DateTime diaMesAno)
        {
            return diaMesAno.AddHours(time.Hours).AddMinutes(time.Minutes);
        }

        public static DateTime ToDateTime(this TimeSpan time)
        {
            return DateTime.Now.Date.AddHours(time.Hours).AddMinutes(time.Minutes);
        }
    }
}
