using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.DataSource.Contract;
using LiveNewsFeed.Models;

namespace LiveNewsFeed.DataSource.DenikNcz
{
    public class CzDenikNDataSource : INewsFeedDataSource
    {
        private const string RootApiUrl = "https://denikn.cz/api/minute";

        private readonly ILogger<CzDenikNDataSource>? _logger;
        private readonly HttpClient _httpClient;

        public CzDenikNDataSource(HttpClient httpClient, ILogger<CzDenikNDataSource>? logger = null)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }

        public Task<IList<NewsArticlePost>> GetPostsAsync(DateTime? before = default,
                                                          DateTime? after = default,
                                                          Category? category = default,
                                                          Tag? tag = default,
                                                          bool? important = default,
                                                          int? count = default)
        {
            
        }
    }
}
