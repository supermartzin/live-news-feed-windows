using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class DataSourcesManager : IDataSourcesManager
    {
        private readonly Dictionary<string, NewsFeedDataSource> _dataSources;

        public DataSourcesManager()
        {
            _dataSources = new Dictionary<string, NewsFeedDataSource>();
        }

        public void RegisterDataSource(NewsFeedDataSource newsFeedDataSource)
        {
            if (newsFeedDataSource == null)
                throw new ArgumentNullException(nameof(newsFeedDataSource));

            if (_dataSources.ContainsKey(newsFeedDataSource.Name))
                throw new ArgumentException("Data source already registered.", nameof(newsFeedDataSource));

            _dataSources[newsFeedDataSource.Name] = newsFeedDataSource;
        }

        public IList<NewsFeedDataSource> GetRegisteredDataSources()
        {
            return _dataSources.Values.ToImmutableList();
        }

        public async Task<IList<NewsArticlePost>> GetLatestPostsFromAllAsync()
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values)
            {
                posts.AddRange(await dataSource.NewsFeed.GetPostsAsync(count: 20).ConfigureAwait(false));
            }

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime).ToList();
        }
    }
}