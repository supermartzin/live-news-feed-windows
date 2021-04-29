using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public abstract class ValueConverterBase : IValueConverter
    {
        private readonly ResourceLoader _resources = ResourceLoader.GetForViewIndependentUse();

        public abstract object Convert(object value, Type targetType, object parameter, string language);

        public abstract object ConvertBack(object value, Type targetType, object parameter, string language);

        protected virtual string GetLocalizedString(string resourceKey) => _resources.GetString(resourceKey);
    }
}