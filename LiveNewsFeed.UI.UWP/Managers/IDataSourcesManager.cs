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

        Task<IList<NewsArticlePost>> GetLatestPostsFromAllAsync();
    }
}