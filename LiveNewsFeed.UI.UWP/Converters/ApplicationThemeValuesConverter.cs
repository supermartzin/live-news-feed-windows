using System;

using LiveNewsFeed.UI.UWP.Managers;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class ApplicationThemeValuesConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string strValue)
                return value;
            if (!Enum.TryParse<Theme>(strValue, out _))
                return value;

            return GetLocalizedString($"AppTheme_{strValue}");
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}