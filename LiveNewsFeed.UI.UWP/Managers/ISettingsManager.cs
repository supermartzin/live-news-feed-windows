using System.Threading.Tasks;

using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface ISettingsManager
    {
        ApplicationSettings ApplicationSettings { get; }

        NotificationSettings NotificationSettings { get; }

        AutomaticUpdateSettings AutomaticUpdateSettings { get; }

        NewsFeedDisplaySettings NewsFeedDisplaySettings { get; }
        
        Task LoadSettingsAsync();

        Task SaveSettingsAsync();
    }
}