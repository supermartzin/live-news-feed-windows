﻿using System;
using System.Globalization;

namespace LiveNewsFeed.UI.UWP.Converters
{
    public class LanguageCodesConverter : ValueConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not string languageCode)
                return value;

            return CultureInfo.GetCultureInfo(languageCode).NativeName;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}