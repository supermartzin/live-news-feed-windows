using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class AutomaticUpdatesIntervalValueConverter : IValueConverter
    {
        private static readonly ResourceLoader Resources = ResourceLoader.GetForViewIndependentUse();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not double interval)
                return value;

            if (interval < 60)
                return $"{interval} {Resources.GetString("General_Seconds")}";
            if (Math.Abs(interval - 60) < 0.001)
                return $"{(int) (interval / 60)} {Resources.GetString("General_Minute")}";
            if (interval is > 60 and < 300)
                return $"{(int) (interval / 60)} {Resources.GetString("General_TwoToFourMinutes")}";
            if (interval >= 300)
                return $"{(int) (interval / 60)} {Resources.GetString("General_Minutes")}";

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}