using System;
using Windows.UI.Xaml;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class InvertVisibilityConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language) => value switch
        {
            Visibility.Visible => Visibility.Collapsed,
            Visibility.Collapsed => Visibility.Visible,
            _ => Visibility.Collapsed
        };

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}