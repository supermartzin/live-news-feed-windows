using System;
using System.Collections.Generic;

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

        private string _applicationThemeName;
        public string ApplicationThemeName
        {
            get => _applicationThemeName;
            set
            {
                var changed = SetProperty(ref _applicationThemeName, value);
                if (changed)
                    _settingsManager.ApplicationSettings.Theme = Enum.Parse<Theme>(value);
            }
        }

        public IList<string> ApplicationThemeNames { get; }

        public SettingsMenuViewModel(ISettingsManager settingsManager)
        {
            _settingsManager = settingsManager ?? throw new ArgumentNullException(nameof(settingsManager));
            ApplicationThemeNames = new List<string>
            {
                nameof(Theme.SystemDefault),
                nameof(Theme.Light),
                nameof(Theme.Dark)
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            _displayLanguageCode = _settingsManager.ApplicationSettings.DisplayLanguageCode;
            _applicationThemeName = _settingsManager.ApplicationSettings.Theme.ToString();
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
                    if (eventArgs.TryGetNewValueAs<string>(out var languageCode) && languageCode != null && DisplayLanguageCode != languageCode)
                        DisplayLanguageCode = languageCode;
                    break;

                case nameof(ApplicationSettings.Theme):
                    if (eventArgs.TryGetNewValueAs<Theme>(out var appTheme) && ApplicationThemeName != appTheme.ToString())
                        ApplicationThemeName = appTheme.ToString();
                    break;

                case nameof(AutomaticUpdateSettings.AutomaticUpdateAllowed):
                    if (eventArgs.TryGetNewValueAs<bool>(out var updatesAllowed) && AutomaticUpdates != updatesAllowed)
                        AutomaticUpdates = updatesAllowed;
                    break;

                case nameof(AutomaticUpdateSettings.UpdateInterval):
                    if (eventArgs.TryGetNewValueAs<TimeSpan>(out var interval) && Math.Abs(AutomaticUpdatesInterval - interval.TotalSeconds) > 0.001)
                        AutomaticUpdatesInterval = interval.TotalSeconds;
                    break;

                case nameof(NotificationSettings.NotificationsAllowed):
                    if (eventArgs.TryGetNewValueAs<bool>(out var allowed) && NotificationsTurnedOn != allowed)
                        NotificationsTurnedOn = allowed;
                    break;

                case nameof(NotificationSettings.NotifyOnlyOnImportantPosts):
                    if (eventArgs.TryGetNewValueAs<bool>(out var onlyOnImportantPosts) && NotifyOnlyOnImportantPosts != onlyOnImportantPosts)
                        NotifyOnlyOnImportantPosts = onlyOnImportantPosts;
                    break;

                case nameof(NewsFeedDisplaySettings.ShowOnlyImportantPosts):
                    if (eventArgs.TryGetNewValueAs<bool>(out var showOnlyImportantPosts) && ShowOnlyImportantPosts != showOnlyImportantPosts)
                        ShowOnlyImportantPosts = showOnlyImportantPosts;
                    break;
            }
        }
    }
}