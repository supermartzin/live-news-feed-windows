using System;
using System.Collections.Generic;
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

        private readonly IList<NewsArticlePost> _posts;

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

        public Task<IList<NewsArticlePost>> GetLatestPostsAsync(int count = 50)
        {
            throw new NotImplementedException();
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
