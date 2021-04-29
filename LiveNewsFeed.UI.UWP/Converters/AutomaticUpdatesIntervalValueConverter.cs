using System;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class AutomaticUpdatesIntervalValueConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not double interval)
                return value;

            if (interval < 60)
                return $"{interval} {GetLocalizedString("General_Seconds")}";
            if (Math.Abs(interval - 60) < 0.001)
                return $"{(int) (interval / 60)} {GetLocalizedString("General_Minute")}";
            if (interval is > 60 and < 300)
                return $"{(int) (interval / 60)} {GetLocalizedString("General_TwoToFourMinutes")}";
            if (interval >= 300)
                return $"{(int) (interval / 60)} {GetLocalizedString("General_Minutes")}";

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}