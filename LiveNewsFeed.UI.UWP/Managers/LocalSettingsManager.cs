﻿using System;
using System.Threading.Tasks;
using Windows.Storage;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class LocalSettingsManager : ISettingsManager
    {
        private static class SettingsKeys
        {
            public const string ApplicationSettingsKey = "appSettings";
            public const string LanguageCodeKey = "language";
            public const string ApplicationThemeKey = "appTheme";
            public const string NotificationSettingsKey = "notificationSettings";
            public const string NotificationsAllowedKey = "allowed";
            public const string NotifyOnlyOnImportantPostsKey = "onlyImportant";
            public const string AutomaticUpdateSettingsKey = "automaticUpdateSettings";
            public const string AutomaticUpdatesAllowedKey = "allowed";
            public const string AutomaticUpdatesIntervalKey = "interval";
            public const string NewsFeedDisplaySettingsKey = "newsFeedDisplaySettings";
            public const string ShowOnlyImportantPostsKey = "onlyImportant";
            public const string NewsFeedDataSourceStates = "newsFeedDataSourceStates";
        }

        private static class DefaultSettings
        {
            public const string DisplayLanguageCode = "en";
            public const Theme ApplicationTheme = Theme.SystemDefault;
            public const bool NotificationsAllowed = false;
            public const bool NotifyOnlyOnImportantPosts = false;
            public const bool AutomaticUpdateAllowed = false;
            public static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(5);
            public const bool ShowOnlyImportantPosts = false;
        }

        private readonly ApplicationDataContainer _appDataSettings;

        public ApplicationSettings ApplicationSettings { get; private set; }

        public NotificationSettings NotificationSettings { get; private set; }

        public AutomaticUpdateSettings AutomaticUpdateSettings { get; private set; }

        public NewsFeedDisplaySettings NewsFeedDisplaySettings { get; private set; }
        
        public LocalSettingsManager()
        {
            _appDataSettings = ApplicationData.Current.LocalSettings;
        }

        public Task LoadSettingsAsync()
        {
            LoadApplicationSettings();
            LoadNotificationSettings();
            LoadAutomaticUpdateSettings();
            LoadNewsFeedDisplaySettings();

            return Task.CompletedTask;
        }
        
        public Task SaveSettingsAsync()
        {
            SaveApplicationSettings();
            SaveNotificationSettings();
            SaveAutomaticUpdateSettings();
            SaveNewsFeedDisplaySettings();

            return Task.CompletedTask;
        }

        
        private void LoadApplicationSettings()
        {
            ApplicationSettings = new ApplicationSettings();

            // load from Local AppData
            if (_appDataSettings.Values[SettingsKeys.ApplicationSettingsKey] is ApplicationDataCompositeValue applicationSettings)
            {
                if (applicationSettings.TryGetValue(SettingsKeys.LanguageCodeKey, out var languageCodeValue) && languageCodeValue is string languageCode and not null)
                {
                    ApplicationSettings.DisplayLanguageCode = languageCode;
                }
                if (applicationSettings.TryGetValue(SettingsKeys.ApplicationThemeKey, out var appThemeValue) && appThemeValue is string appTheme and not null)
                {
                    ApplicationSettings.Theme = Enum.Parse<Theme>(appTheme);
                }
            }
            else
            {
                ApplicationSettings.DisplayLanguageCode = DefaultSettings.DisplayLanguageCode;
                ApplicationSettings.Theme = DefaultSettings.ApplicationTheme;

                SaveApplicationSettings();
            }

            ApplicationSettings.SettingChanged += (_, _) => SaveApplicationSettings();
        }

        private void LoadNotificationSettings()
        {
            NotificationSettings = new NotificationSettings();

            // load from Local AppData
            if (_appDataSettings.Values[SettingsKeys.NotificationSettingsKey] is ApplicationDataCompositeValue notificationSettings)
            {
                if (notificationSettings.TryGetValue(SettingsKeys.NotificationsAllowedKey, out var allowedValue) && allowedValue is bool allowed)
                {
                    NotificationSettings.NotificationsAllowed = allowed;
                }
                if (notificationSettings.TryGetValue(SettingsKeys.NotifyOnlyOnImportantPostsKey, out var onlyImportantValue) && onlyImportantValue is bool onlyImportant)
                {
                    NotificationSettings.NotifyOnlyOnImportantPosts = onlyImportant;
                }
            }
            else
            {
                NotificationSettings.NotificationsAllowed = DefaultSettings.NotificationsAllowed;
                NotificationSettings.NotifyOnlyOnImportantPosts = DefaultSettings.NotifyOnlyOnImportantPosts;

                SaveNotificationSettings();
            }

            NotificationSettings.SettingChanged += (_, _) => SaveNotificationSettings();
        }

        private void LoadAutomaticUpdateSettings()
        {
            AutomaticUpdateSettings = new AutomaticUpdateSettings();

            // load from Local AppData
            if (_appDataSettings.Values[SettingsKeys.AutomaticUpdateSettingsKey] is ApplicationDataCompositeValue automaticUpdateSettings)
            {
                if (automaticUpdateSettings.TryGetValue(SettingsKeys.AutomaticUpdatesAllowedKey, out var value) && value is bool allowed)
                {
                    AutomaticUpdateSettings.AutomaticUpdateAllowed = allowed;
                }
                if (automaticUpdateSettings.TryGetValue(SettingsKeys.AutomaticUpdatesIntervalKey, out value) && value is string intervalString)
                {
                    if (TimeSpan.TryParse(intervalString, out var interval))
                        AutomaticUpdateSettings.UpdateInterval = interval;
                }
            }
            else
            {
                AutomaticUpdateSettings.AutomaticUpdateAllowed = DefaultSettings.AutomaticUpdateAllowed;
                AutomaticUpdateSettings.UpdateInterval = DefaultSettings.UpdateInterval;

                SaveAutomaticUpdateSettings();
            }

            AutomaticUpdateSettings.SettingChanged += (_, _) => SaveAutomaticUpdateSettings();
        }

        private void LoadNewsFeedDisplaySettings()
        {
            NewsFeedDisplaySettings = new NewsFeedDisplaySettings();

            // load from Local AppData
            if (_appDataSettings.Values[SettingsKeys.NewsFeedDisplaySettingsKey] is ApplicationDataCompositeValue newsFeedDisplaySettings)
            {
                if (newsFeedDisplaySettings.TryGetValue(SettingsKeys.ShowOnlyImportantPostsKey, out var value) && value is bool onlyImportant)
                {
                    NewsFeedDisplaySettings.ShowOnlyImportantPosts = onlyImportant;
                }
            }
            else
            {
                NewsFeedDisplaySettings.ShowOnlyImportantPosts = DefaultSettings.ShowOnlyImportantPosts;

                SaveNewsFeedDisplaySettings();
            }
            if (_appDataSettings.Values[SettingsKeys.NewsFeedDataSourceStates] is ApplicationDataCompositeValue newsFeedDataSourceStates)
            {
                foreach (var (name, state) in newsFeedDataSourceStates)
                {
                    if (state is bool isEnabled)
                    {
                        NewsFeedDisplaySettings.SetNewsFeedDataSourceState(name, isEnabled);
                    }
                }
            }

            NewsFeedDisplaySettings.SettingChanged += (_, _) => SaveNewsFeedDisplaySettings();
        }

        private void SaveApplicationSettings()
        {
            // save to Local AppData
            _appDataSettings.Values[SettingsKeys.ApplicationSettingsKey] = new ApplicationDataCompositeValue
            {
                [SettingsKeys.LanguageCodeKey] = ApplicationSettings.DisplayLanguageCode,
                [SettingsKeys.ApplicationThemeKey] = ApplicationSettings.Theme.ToString()
            };
        }

        private void SaveNotificationSettings()
        {
            // save to Local AppData
            _appDataSettings.Values[SettingsKeys.NotificationSettingsKey] = new ApplicationDataCompositeValue
            {
                [SettingsKeys.NotificationsAllowedKey] = NotificationSettings.NotificationsAllowed,
                [SettingsKeys.NotifyOnlyOnImportantPostsKey] = NotificationSettings.NotifyOnlyOnImportantPosts
            };
        }

        private void SaveAutomaticUpdateSettings()
        {
            // save to Local AppData
            _appDataSettings.Values[SettingsKeys.AutomaticUpdateSettingsKey] = new ApplicationDataCompositeValue
            {
                [SettingsKeys.AutomaticUpdatesAllowedKey] = AutomaticUpdateSettings.AutomaticUpdateAllowed,
                [SettingsKeys.AutomaticUpdatesIntervalKey] = AutomaticUpdateSettings.UpdateInterval.ToString()
            };
        }

        private void SaveNewsFeedDisplaySettings()
        {
            var newsFeedDataSourceStates = new ApplicationDataCompositeValue();
            foreach (var (name, isEnabled) in NewsFeedDisplaySettings.NewsFeedDataSourceStates)
            {
                newsFeedDataSourceStates[name] = isEnabled;
            }

            // save to Local AppData
            _appDataSettings.Values[SettingsKeys.NewsFeedDataSourceStates] = newsFeedDataSourceStates;
            _appDataSettings.Values[SettingsKeys.NewsFeedDisplaySettingsKey] = new ApplicationDataCompositeValue
            {
                [SettingsKeys.ShowOnlyImportantPostsKey] = NewsFeedDisplaySettings.ShowOnlyImportantPosts
            };
        }
    }
}