using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.Models;
using LiveNewsFeed.DataSource.Common;

namespace LiveNewsFeed.DataSource.DenikNcz
{
    public class DenikNczNewsFeed : INewsFeed
    {
        private const string RootApiUrl = "https://denikn.cz/api/minute";

        private readonly ILogger<DenikNczNewsFeed>? _logger;
        private readonly HttpClient _httpClient;

        public string Name => "Deník N";

        public DenikNczNewsFeed(HttpClient httpClient, ILogger<DenikNczNewsFeed>? logger = null)
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
            throw new NotImplementedException();
        }
    }
}
