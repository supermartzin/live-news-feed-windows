using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Services
{
    public interface ILiveTileService
    {
        void UpdateLiveTile(NewsArticlePost newsArticlePost, bool skipIfQueueIsFull = false);
    }
}