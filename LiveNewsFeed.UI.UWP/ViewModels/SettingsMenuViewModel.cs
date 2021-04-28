using System;

using LiveNewsFeed.UI.UWP.Managers;
using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.ViewModels
{
    public class SettingsMenuViewModel : ViewModelBase
    {
        private readonly ISettingsManager _settingsManager;

        private bool _automaticUpdates;
        public bool AutomaticUpdates
        {
            get => _automaticUpdates;
            set
            {
                var changed = SetProperty(ref _automaticUpdates, value);
                if (changed)
                    _settingsManager.AutomaticUpdateSettings.AutomaticUpdateAllowed = value;
            }
        }

        private double _automaticUpdatesInterval;
        public double AutomaticUpdatesInterval
        {
            get => _automaticUpdatesInterval;
            set
            {
                var changed = SetProperty(ref _automaticUpdatesInterval, value);
                if (changed)
                    _settingsManager.AutomaticUpdateSettings.UpdateInterval = TimeSpan.FromSeconds(value);
            }
        }

        private bool _notificationsTurnedOn;
        public bool NotificationsTurnedOn
        {
            get => _notificationsTurnedOn;
            set
            {
                var changed = SetProperty(ref _notificationsTurnedOn, value);
                if (changed)
                    _settingsManager.NotificationSettings.NotificationsAllowed = value;
            }
        }

        private bool _notifyOnlyOnImportantPosts;
        public bool NotifyOnlyOnImportantPosts
        {
            get => _notifyOnlyOnImportantPosts;
            set
            {
                var changed = SetProperty(ref _notifyOnlyOnImportantPosts, value);
                if (changed)
                    _settingsManager.NotificationSettings.NotifyOnlyOnImportantPosts = value;
            }
        }

        private bool _showOnlyImportantPosts;
        public bool ShowOnlyImportantPosts
        {
            get => _showOnlyImportantPosts;
            set
            {
                var changed = SetProperty(ref _showOnlyImportantPosts, value);
                if (changed)
                    _settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts = value;
            }
        }

        private string _displayLanguageCode;
        public string DisplayLanguageCode
        {
            get => _displayLanguageCode;
            set
            {
                var changed = SetProperty(ref _displayLanguageCode, value);
                if (changed) 
                    _settingsManager.ApplicationSettings.DisplayLanguageCode = value;
            }
        }

        public SettingsMenuViewModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));

            LoadSettings();
        }

        private void LoadSettings()
        {
            _displayLanguageCode = _settingsManager.ApplicationSettings.DisplayLanguageCode;
            _automaticUpdates = _settingsManager.AutomaticUpdateSettings.AutomaticUpdateAllowed;
            _automaticUpdatesInterval = _settingsManager.AutomaticUpdateSettings.UpdateInterval.TotalSeconds;
            _notificationsTurnedOn = _settingsManager.NotificationSettings.NotificationsAllowed;
            _notifyOnlyOnImportantPosts = _settingsManager.NotificationSettings.NotifyOnlyOnImportantPosts;
            _showOnlyImportantPosts = _settingsManager.NewsFeedDisplaySettings.ShowOnlyImportantPosts;

            _settingsManager.ApplicationSettings.SettingChanged += Settings_OnChanged;
            _settingsManager.AutomaticUpdateSettings.SettingChanged += Settings_OnChanged;
            _settingsManager.NotificationSettings.SettingChanged += Settings_OnChanged;
            _settingsManager.NewsFeedDisplaySettings.SettingChanged += Settings_OnChanged;
        }

        private void Settings_OnChanged(object sender, SettingChangedEventArgs eventArgs)
        {
            switch (eventArgs.SettingName)
            {
                case nameof(ApplicationSettings.DisplayLanguageCode):
                    if (eventArgs.TryGetNewValue<string>(out var languageCode) && DisplayLanguageCode != languageCode && languageCode != null)
                        DisplayLanguageCode = languageCode;
                    break;

                case nameof(AutomaticUpdateSettings.AutomaticUpdateAllowed):
                    if (eventArgs.TryGetNewValue<bool>(out var updatesAllowed) && AutomaticUpdates != updatesAllowed)
                        AutomaticUpdates = updatesAllowed;
                    break;

                case nameof(AutomaticUpdateSettings.UpdateInterval):
                    if (eventArgs.TryGetNewValue<TimeSpan>(out var interval) && Math.Abs(AutomaticUpdatesInterval - interval.TotalSeconds) > 0.001)
                        AutomaticUpdatesInterval = interval.TotalSeconds;
                    break;

                case nameof(NotificationSettings.NotificationsAllowed):
                    if (eventArgs.TryGetNewValue<bool>(out var allowed) && NotificationsTurnedOn != allowed)
                        NotificationsTurnedOn = allowed;
                    break;

                case nameof(NotificationSettings.NotifyOnlyOnImportantPosts):
                    if (eventArgs.TryGetNewValue<bool>(out var onlyOnImportantPosts) && NotifyOnlyOnImportantPosts != onlyOnImportantPosts)
                        NotifyOnlyOnImportantPosts = onlyOnImportantPosts;
                    break;

                case nameof(NewsFeedDisplaySettings.ShowOnlyImportantPosts):
                    if (eventArgs.TryGetNewValue<bool>(out var showOnlyImportantPosts) && ShowOnlyImportantPosts != showOnlyImportantPosts)
                        ShowOnlyImportantPosts = showOnlyImportantPosts;
                    break;
            }
        }
    }
}