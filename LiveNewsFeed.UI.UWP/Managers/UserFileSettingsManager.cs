using System;
using System.Threading.Tasks;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class UserFileSettingsManager : ISettingsManager
    {
        public NotificationSettings NotificationSettings { get; private set; }

        public AutomaticUpdateSettings AutomaticUpdateSettings { get; private set; }

        public NewsFeedDisplaySettings NewsFeedDisplaySettings { get; private set; }

        public bool AreSettingsLoaded { get; private set; }

        public event EventHandler? SettingsLoaded;

        public Task LoadSettingsAsync()
        {
            NotificationSettings = new NotificationSettings();
            AutomaticUpdateSettings = new AutomaticUpdateSettings();
            NewsFeedDisplaySettings = new NewsFeedDisplaySettings();

            NotificationSettings.SettingChanged += OnSettingsChanged;
            AutomaticUpdateSettings.SettingChanged += OnSettingsChanged;
            NewsFeedDisplaySettings.SettingChanged += OnSettingsChanged;

            AreSettingsLoaded = true;

            SettingsLoaded?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }
        

        private Task SaveSettingsAsync()
        {
            return Task.CompletedTask;
        }

        private void OnSettingsChanged(object sender, SettingChangedEventArgs e)
        {
            // TODO save settings
        }
    }
}