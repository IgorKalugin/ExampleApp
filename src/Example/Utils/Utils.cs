using System;
using Xamarin.Forms;

namespace Example.Utils
{
    public static class Utils
    {
        public static DateTime TrimToMinutes(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public static int ToAndroidColor(this Color color)
        {
            return (byte)(byte.MaxValue * color.A) << 24 | (byte)(byte.MaxValue * color.R) << 16 | 
                   (byte)(byte.MaxValue * color.G) << 8 | (byte)(byte.MaxValue * color.B);
        }

        public static string ToStringWithTimeZone(this DateTimeOffset? dateTime)
        {
            return !dateTime.HasValue ? "null" : $"{dateTime.Value.LocalDateTime:dd.MM.yyyy HH:mm:ss zz}";
        }
    }
}