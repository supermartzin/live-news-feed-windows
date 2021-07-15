using System;
using System.Globalization;

namespace LiveNewsFeed.DataSource.SeznamZpravyCzNewsFeed
{
    internal static class TypeConverter
    {
        public static DateTime ToDateTime(string value, DateTime defaultValue = default) => DateTime.TryParse(value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out var parsed)
            ? parsed.ToLocalTime()
            : defaultValue;

        public static int ToInt(string value, int defaultValue = 0) => int.TryParse(value, out var number) ? number : defaultValue;
    }
}