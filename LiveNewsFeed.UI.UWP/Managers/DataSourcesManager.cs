using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class DataSourcesManager : IDataSourcesManager
    {
        private readonly Dictionary<string, NewsFeedDataSource> _dataSources;
        private readonly Dictionary<string, DateTime> _dataSourcesLastPostPublishTimes;
        
        public DataSourcesManager()
        {
            _dataSources = new Dictionary<string, NewsFeedDataSource>();
            _dataSourcesLastPostPublishTimes = new Dictionary<string, DateTime>();
        }

        public void RegisterDataSource(NewsFeedDataSource newsFeedDataSource)
        {
            if (newsFeedDataSource == null)
                throw new ArgumentNullException(nameof(newsFeedDataSource));

            if (_dataSources.ContainsKey(newsFeedDataSource.Name))
                throw new ArgumentException("Data source already registered.", nameof(newsFeedDataSource));

            _dataSources[newsFeedDataSource.Name] = newsFeedDataSource;

            // register logo
            Helpers.RegisterLogoForNewsFeed(newsFeedDataSource.Name, new ImageBrush
            {
                ImageSource = new BitmapImage(newsFeedDataSource.LogoUrl)
            });

            // cache logo in memory
            ImageCache.Instance.PreCacheAsync(newsFeedDataSource.LogoUrl, false, true);
        }

        public IList<NewsFeedDataSource> GetRegisteredDataSources()
        {
            return _dataSources.Values.ToImmutableList();
        }

        public NewsFeedDataSource? GetDataSourceByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _dataSources.ContainsKey(name) ? _dataSources[name] : default;
        }

        public async Task<IList<NewsArticlePost>> GetLatestPostsFromAllAsync()
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values)
            {
                var newPosts = await dataSource.NewsFeed.GetPostsAsync().ConfigureAwait(false);

                // set last update time
                var latestPost = newPosts.FirstOrDefault();
                if (latestPost != null)
                    _dataSourcesLastPostPublishTimes[dataSource.Name] = latestPost.PublishTime;

                posts.AddRange(newPosts);
            }

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime).ToList();
        }

        public async Task<IList<NewsArticlePost>> GetLatestPostsSinceLastUpdateAsync()
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values)
            {
                // get last update time
                DateTime? lastPostPublishTime = _dataSourcesLastPostPublishTimes[dataSource.Name];
                if (lastPostPublishTime == DateTime.MinValue)
                    lastPostPublishTime = default;

                var newPosts = await dataSource.NewsFeed.GetPostsAsync(after: lastPostPublishTime).ConfigureAwait(false);

                // set last update time
                var latestPost = newPosts.FirstOrDefault();
                if (latestPost != null)
                    _dataSourcesLastPostPublishTimes[dataSource.Name] = latestPost.PublishTime;

                posts.AddRange(newPosts);
            }
            
            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime).ToList();
        }
    }
}