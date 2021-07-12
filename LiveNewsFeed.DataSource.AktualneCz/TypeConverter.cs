using System;

namespace LiveNewsFeed.DataSource.AktualneCz
{
    internal static class TypeConverter
    {
        public static DateTime ToDateTime(string value, DateTime defaultValue = default) => DateTime.TryParse(value, out var parsed) ? parsed : defaultValue;

        public static int ToInt(string value, int defaultValue = 0) => int.TryParse(value, out var number) ? number : defaultValue;
    }
}