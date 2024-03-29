﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

using LiveNewsFeed.UI.UWP.Common;

namespace LiveNewsFeed.UI.UWP.Managers
{
    public class DataSourcesManager : IDataSourcesManager
    {
        private readonly ILogger<DataSourcesManager>? _logger;

        private readonly object _updateLock = new();
        
        private readonly Dictionary<string, NewsFeedDataSource> _dataSources;
        private readonly Dictionary<string, DateTime> _dataSourcesLatestPostPublishTimes;
        private readonly Dictionary<string, DateTime> _dataSourcesOldestPostPublishTimes;

        public event EventHandler<NewsArticlePost>? NewsArticlePostReceived;

        public DataSourcesManager(ILogger<DataSourcesManager>? logger = null)
        {
            _dataSources = new Dictionary<string, NewsFeedDataSource>();
            _dataSourcesLatestPostPublishTimes = new Dictionary<string, DateTime>();
            _dataSourcesOldestPostPublishTimes = new Dictionary<string, DateTime>();
            _logger = logger;
        }

        public void RegisterDataSource(NewsFeedDataSource newsFeedDataSource)
        {
            if (newsFeedDataSource == null)
                throw new ArgumentNullException(nameof(newsFeedDataSource));

            if (_dataSources.ContainsKey(newsFeedDataSource.Name))
                throw new ArgumentException(@"Data source already registered.", nameof(newsFeedDataSource));

            _dataSources[newsFeedDataSource.Name] = newsFeedDataSource;

            _logger?.LogInformation($"Registered News Feed data source '{newsFeedDataSource.Name}'.");

            // register logos
            Helpers.RegisterLogoForNewsFeed(newsFeedDataSource.Name, ApplicationTheme.Light, newsFeedDataSource.Logo.LightThemeUrl);
            Helpers.RegisterLogoForNewsFeed(newsFeedDataSource.Name, ApplicationTheme.Dark, newsFeedDataSource.Logo.DarkThemeUrl);
        }

        public IEnumerable<NewsFeedDataSource> GetRegisteredDataSources()
        {
            return _dataSources.Values.ToImmutableList();
        }

        public NewsFeedDataSource? GetDataSourceByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            return _dataSources.ContainsKey(name) ? _dataSources[name] : default;
        }

        public async Task<IEnumerable<NewsArticlePost>> GetLatestPostsFromAllAsync(DataSourceUpdateOptions? options = default)
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values.Where(dataSource => dataSource.IsEnabled))
            {
                var newPosts = await dataSource.NewsFeed
                                               .GetPostsAsync(options?.Before,
                                                              options?.After,
                                                              options?.Category,
                                                              options?.Tag,
                                                              options?.Important,
                                                              options?.Count).ConfigureAwait(false);

                newPosts = newPosts.OrderBy(post => post.PublishTime).ToList();

                lock (_updateLock)
                {
                    // set last update time
                    var latestPost = newPosts.LastOrDefault();
                    if (latestPost != null)
                        _dataSourcesLatestPostPublishTimes[dataSource.Name] = latestPost.PublishTime;

                    // set oldest post time
                    var oldestPost = newPosts.FirstOrDefault();
                    if (oldestPost != null)
                        _dataSourcesOldestPostPublishTimes[dataSource.Name] = oldestPost.PublishTime;
                }

                posts.AddRange(newPosts);
            }

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime);
        }

        public async Task<IEnumerable<NewsArticlePost>> GetLatestPostsSinceLastUpdateAsync(DataSourceUpdateOptions? options = default)
        {
            return await LatestPostsSinceLastUpdateAsync(options).ConfigureAwait(false);
        }

        public async Task<IEnumerable<NewsArticlePost>> GetOlderPostsFromAllAsync(DataSourceUpdateOptions? options = default)
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values.Where(dataSource => dataSource.IsEnabled))
            {
                // get oldest post time
                DateTime oldestPostTime;
                lock (_updateLock)
                {
                    if (!_dataSourcesOldestPostPublishTimes.TryGetValue(dataSource.Name, out oldestPostTime))
                        oldestPostTime = DateTime.Now;
                }

                _logger?.LogDebug($"Loading older posts from '{dataSource.Name}' before {oldestPostTime}.");

                var newPosts = await dataSource.NewsFeed
                                               .GetPostsAsync(options?.Before ?? oldestPostTime,
                                                              options?.After,
                                                              options?.Category,
                                                              options?.Tag,
                                                              options?.Important,
                                                              options?.Count).ConfigureAwait(false);

                newPosts = newPosts.OrderBy(post => post.PublishTime).ToList();

                lock (_updateLock)
                {
                    // set oldest post time
                    var oldestPost = newPosts.FirstOrDefault();
                    if (oldestPost != null)
                        _dataSourcesOldestPostPublishTimes[dataSource.Name] = oldestPost.PublishTime;
                }

                posts.AddRange(newPosts);
            }

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime);
        }

        public async Task LoadLatestPostsSinceLastUpdateAsync(DataSourceUpdateOptions? options = default)
        {
            var posts = (await LatestPostsSinceLastUpdateAsync(options).ConfigureAwait(false)).ToList();

            _logger?.LogDebug($"Downloaded {posts.Count} posts since last update");

            // raise events
            foreach (var post in posts)
            {
                NewsArticlePostReceived?.Invoke(this, post);
            }
        }


        private async Task<IEnumerable<NewsArticlePost>> LatestPostsSinceLastUpdateAsync(DataSourceUpdateOptions? options = default)
        {
            var posts = new List<NewsArticlePost>();

            foreach (var dataSource in _dataSources.Values.Where(dataSource => dataSource.IsEnabled))
            {
                // get last update time
                DateTime lastPostPublishTime;
                lock (_updateLock)
                {
                    if (!_dataSourcesLatestPostPublishTimes.TryGetValue(dataSource.Name, out lastPostPublishTime))
                        lastPostPublishTime = DateTime.MinValue;
                }

                var newPosts = await dataSource.NewsFeed
                                               .GetPostsAsync(options?.Before,
                                                              options?.After ?? lastPostPublishTime,
                                                              options?.Category,
                                                              options?.Tag,
                                                              options?.Important,
                                                              options?.Count).ConfigureAwait(false);

                newPosts = newPosts.OrderBy(post => post.PublishTime).ToList();

                lock (_updateLock)
                {
                    // set last update time
                    var latestPost = newPosts.LastOrDefault();
                    if (latestPost != null)
                        _dataSourcesLatestPostPublishTimes[dataSource.Name] = latestPost.PublishTime;
                }

                posts.AddRange(newPosts);
            }

            // order and return posts
            return posts.OrderByDescending(post => post.PublishTime);
        }
    }
}