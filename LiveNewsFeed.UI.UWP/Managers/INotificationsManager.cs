using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface INotificationsManager
    {
        NotificationSettings Settings { get; }

        void ShowNotification(NewsArticlePost articlePost);
    }
}