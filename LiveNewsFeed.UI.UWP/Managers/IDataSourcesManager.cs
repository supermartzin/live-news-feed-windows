using System.Collections.Generic;
using System.Threading.Tasks;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IDataSourcesManager
    {
        void RegisterDataSource(NewsFeedDataSource newsFeedDataSource);

        IList<NewsFeedDataSource> GetRegisteredDataSources();

        NewsFeedDataSource? GetDataSourceByName(string name);

        Task<IList<NewsArticlePost>> GetLatestPostsFromAllAsync();

        Task<IList<NewsArticlePost>> GetLatestPostsSinceLastUpdateAsync();
    }
}