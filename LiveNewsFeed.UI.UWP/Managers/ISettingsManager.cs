using System;
using System.Threading.Tasks;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface ISettingsManager
    {
        NotificationSettings NotificationSettings { get; }

        AutomaticUpdateSettings AutomaticUpdateSettings { get; }

        NewsFeedDisplaySettings NewsFeedDisplaySettings { get; }

        bool AreSettingsLoaded { get; }

        event EventHandler? SettingsLoaded;

        Task LoadSettingsAsync();

        Task SaveSettingsAsync();
    }
}