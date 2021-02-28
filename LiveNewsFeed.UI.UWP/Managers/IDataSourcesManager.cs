using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public interface IDataSourcesManager
    {
        event EventHandler<NewsArticlePost> NewsArticlePostReceived;
        
        void RegisterDataSource(NewsFeedDataSource newsFeedDataSource);

        IEnumerable<NewsFeedDataSource> GetRegisteredDataSources();

        NewsFeedDataSource? GetDataSourceByName(string name);

        Task<IEnumerable<NewsArticlePost>> GetLatestPostsFromAllAsync(DataSourceUpdateOptions? options = default);

        Task<IEnumerable<NewsArticlePost>> GetLatestPostsSinceLastUpdateAsync(DataSourceUpdateOptions? options = default);

        Task LoadLatestPostsSinceLastUpdateAsync(DataSourceUpdateOptions? options = default);
    }
}