using System.Collections.Generic;

using LiveNewsFeed.Models;
using LiveNewsFeed.UI.UWP.Managers.Settings;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface INotificationsManager
    {
        NotificationSettings Settings { get; }

        Dictionary<string, NewsArticlePost> NotifiedPosts { get; }

        void ShowNotification(NewsArticlePost articlePost);
    }
}