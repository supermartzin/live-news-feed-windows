﻿namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class ApplicationSettings : SettingsBase
    {
        private string _displayLanguageCode = string.Empty;
        public string DisplayLanguageCode
        {
            get => _displayLanguageCode;
            set => Set(ref _displayLanguageCode, value);
        }

        private Theme _theme;
        public Theme Theme
        {
            get => _theme;
            set => Set(ref _theme, value);
        }
    }
}