using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class LanguageCodesConverter : IValueConverter
    {
        private static readonly ResourceLoader Resources = ResourceLoader.GetForViewIndependentUse();

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string languageCode)
                return value;

            return CultureInfo.GetCultureInfo(languageCode).NativeName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}