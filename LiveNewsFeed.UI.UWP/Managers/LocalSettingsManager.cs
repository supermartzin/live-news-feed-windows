using System;
using System.Threading.Tasks;
using Windows.Storage;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class LocalSettingsManager : ISettingsManager
    {
        private static class SettingsKeys
        {
            public const string NotificationSettingsKey = "notificationSettings";
            public const string NotificationsAllowedKey = "allowed";
            public const string NotifyOnlyOnImportantPostsKey = "onlyImportant";
            public const string AutomaticUpdateSettingsKey = "automaticUpdateSettings";
            public const string AutomaticUpdatesAllowedKey = "allowed";
            public const string AutomaticUpdatesIntervalKey = "interval";
            public const string NewsFeedDisplaySettingsKey = "newsFeedDisplaySettings";
            public const string ShowOnlyImportantPostsKey = "onlyImportant";
        }

        private readonly ApplicationDataContainer _appDataSettings;

        public NotificationSettings NotificationSettings { get; private set; }

        public AutomaticUpdateSettings AutomaticUpdateSettings { get; private set; }

        public NewsFeedDisplaySettings NewsFeedDisplaySettings { get; private set; }

        public bool AreSettingsLoaded { get; private set; }

        public event EventHandler? SettingsLoaded;

        public LocalSettingsManager()
        {
            _appDataSettings = ApplicationData.Current.LocalSettings;
        }

        public Task LoadSettingsAsync()
        {
            LoadNotificationSettings();
            LoadAutomaticUpdateSettings();
            LoadNewsFeedDisplaySettings();

            AreSettingsLoaded = true;

            SettingsLoaded?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }

        public Task SaveSettingsAsync()
        {
            SaveNotificationSettings();
            SaveAutomaticUpdateSettings();
            SaveNewsFeedDisplaySettings();

            return Task.CompletedTask;
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

            NewsFeedDisplaySettings.SettingChanged += (_, _) => SaveNewsFeedDisplaySettings();
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
            // save to Local AppData
            _appDataSettings.Values[SettingsKeys.NewsFeedDisplaySettingsKey] = new ApplicationDataCompositeValue
            {
                [SettingsKeys.ShowOnlyImportantPostsKey] = NewsFeedDisplaySettings.ShowOnlyImportantPosts
            };
        }
    }
}