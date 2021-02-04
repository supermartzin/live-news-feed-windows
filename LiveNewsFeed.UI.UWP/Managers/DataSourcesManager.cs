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
        
        private DateTime _lastUpdate;

        public DataSourcesManager()
        {
            _dataSources = new Dictionary<string, NewsFeedDataSource>();
            _lastUpdate = DateTime.MinValue;
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

        public NewsFeedDataSource GetDataSourceByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _dataSources.ContainsKey(name) ? _dataSources[name] : default;
        }

        public async Task<IList<NewsArticlePost>> GetLatestPostsFromAllAsync() => await GetPostsAsync().ConfigureAwait(false);

        public async Task<IList<NewsArticlePost>> GetLatestPostsSinceLastUpdateAsync()
        {
            if (_lastUpdate == DateTime.MinValue)
                return await GetLatestPostsFromAllAsync().ConfigureAwait(false);

            return await GetPostsAsync(_lastUpdate).ConfigureAwait(false);
        }
        

        private async Task<IList<NewsArticlePost>> GetPostsAsync(DateTime? after = null)
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values)
            {
                posts.AddRange(await dataSource.NewsFeed.GetPostsAsync(after: after).ConfigureAwait(false));
            }

            _lastUpdate = DateTime.Now;

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime).ToList();
        }
    }
}