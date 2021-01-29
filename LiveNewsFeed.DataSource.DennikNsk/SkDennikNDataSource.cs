using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.DataSource.Contract;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    public class SkDennikNDataSource : INewsFeedDataSource
    {
        private readonly ILogger<SkDennikNDataSource>? _logger;
        private readonly SkDennikNApiDownloader _downloader;
        private readonly string _rootApiUrl;

        private IList<NewsArticlePost> _posts;

        private DateTime _lastUpdateTime;

        public SkDennikNDataSource(string rootApiUrl,
                                   HttpClient httpClient,
                                   ILogger<SkDennikNDataSource>? logger = null)
        {
            _rootApiUrl = rootApiUrl ?? throw new ArgumentNullException(nameof(rootApiUrl));
            _logger = logger;

            _downloader = new SkDennikNApiDownloader(httpClient);
            _lastUpdateTime = DateTime.MinValue;
            _posts = new List<NewsArticlePost>();
        }

        public async Task<IList<NewsArticlePost>> GetLatestPostsAsync(int count = 50)
        {
            try
            {
                var postsDtos = await _downloader.DownloadPostsAsync(_rootApiUrl, count).ConfigureAwait(false);

                _posts = _posts.Union(postsDtos.Select(ModelsConverter.ToNewsArticlePost))
                               .OrderBy(post => post.PublishTime)
                               .ToList();

                // update last update time
                _lastUpdateTime = DateTime.Now;

                return new ReadOnlyCollection<NewsArticlePost>(_posts);
            }
            catch (DownloadException dEx)
            {
                _logger?.LogError(dEx, $"Error getting latest posts from Dennik N - {dEx.Message}");

                return new List<NewsArticlePost>();
            }
        }
        
        public Task<IList<NewsArticlePost>> GetLatestPostsAsync(Category category, int count = 50)
        {
            throw new NotImplementedException();
        }

        public Task<IList<NewsArticlePost>> GetLatestPostsAsync(Tag tag, int count = 50)
        {
            throw new NotImplementedException();
        }

        public Task<IList<NewsArticlePost>> GetLatestImportantPostsAsync(int count = 50)
        {
            throw new NotImplementedException();
        }

        public Task<IList<NewsArticlePost>> GetNewPostsSinceLastUpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}
