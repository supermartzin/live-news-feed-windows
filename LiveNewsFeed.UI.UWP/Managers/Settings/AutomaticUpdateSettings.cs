using System;

namespace LiveNewsFeed.UI.UWP.Managers.Settings
{
    public class AutomaticUpdateSettings : SettingsBase
    {
        private bool _automaticUpdateAllowed;
        public bool AutomaticUpdateAllowed
        {
            get => _automaticUpdateAllowed;
            set => Set(ref _automaticUpdateAllowed, value);
        }

        private TimeSpan _updateInterval;
        public TimeSpan UpdateInterval
        {
            get => _updateInterval;
            set => Set(ref _updateInterval, value);
        }
    }
}