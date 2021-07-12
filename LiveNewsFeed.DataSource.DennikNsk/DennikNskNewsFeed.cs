using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.Models;

using LiveNewsFeed.DataSource.DennikNsk.Converters;
using LiveNewsFeed.DataSource.DennikNsk.DTO;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    public class DennikNskNewsFeed : INewsFeed
    {
        private const string RootApiUrl = "https://dennikn.sk/api/minute";

        private readonly ILogger<DennikNskNewsFeed>? _logger;
        private readonly HttpClient _httpClient;

        public string Name => "Denník N [SK]";

        public DennikNskNewsFeed(HttpClient httpClient, ILogger<DennikNskNewsFeed>? logger = default)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger;
        }
        
        public async Task<IList<NewsArticlePost>> GetPostsAsync(DateTime? before = default,
                                                                DateTime? after = default,
                                                                Category? category = default,
                                                                Tag? tag = default,
                                                                bool? important = default,
                                                                int? count = default)
        {
            try
            {
                var url = BuildUrl(before, after, category, tag, important);

                var postsDtos = await DownloadPostsAsync(url, count ?? 0).ConfigureAwait(false);

                // convert to model
                var posts = postsDtos.Select(dto => ModelsConverter.ToNewsArticlePost(dto, Name))
                                                                   .OrderByDescending(post => post.PublishTime)
                                                                   .ToList();

                if (count > 0 && posts.Count > 0 && posts.Count < count)
                {
                    // get missing number of posts
                    var anotherPosts = await GetPostsAsync(posts.Last().PublishTime,
                                                           after, category, tag, important,
                                                           count - posts.Count).ConfigureAwait(false);
                    posts = posts.Union(anotherPosts)
                                 .OrderByDescending(post => post.PublishTime)
                                 .ToList();
                }

                // API workaround for important posts
                if (important is true && after is not null)
                {
                    posts = posts.Where(post => post.PublishTime > after).ToList();
                }
                if (important is true && before is not null)
                {
                    posts = posts.Where(post => post.PublishTime < before).ToList();
                }

                return posts;
            }
            catch (HttpRequestException hrEx)
            {
                _logger?.LogError(hrEx, $"Error getting posts from {Name}: {hrEx.Message}");

                return new List<NewsArticlePost>();
            }
            catch (JsonException jsonEx)
            {
                _logger?.LogError(jsonEx, $"Error parsing received data from {Name}: {jsonEx.Message}");

                return new List<NewsArticlePost>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting posts from {Name}: {ex.Message}");

                return new List<NewsArticlePost>();
            }
        }


        private static string BuildUrl(DateTime? before, DateTime? after, Category? category, Tag? tag, bool? important)
        {
            var parameters = "";

            if (before.HasValue && important is false)
                parameters += $"before={Uri.EscapeDataString(before.Value.ToUniversalTime().ToString(Constants.DateTimeFormat))}&";
            if (after.HasValue && important is false)
                parameters += $"after={Uri.EscapeDataString(after.Value.ToUniversalTime().ToString(Constants.DateTimeFormat))}&";
            if (category.HasValue)
                parameters += $"cat={ModelsConverter.ToCode(category.Value)}&";
            if (important is true)
                parameters += "important=1&";
            if (tag != null)
            {
                var code = ModelsConverter.ToCode(tag);
                if (code > 0)
                    parameters += $"tag={code}";
            }

            parameters = parameters.TrimEnd('&');

            return !string.IsNullOrEmpty(parameters)
                        ? $"{RootApiUrl}?{parameters}"
                        : RootApiUrl;
        }

        private async Task<IList<ArticlePostDTO>> DownloadPostsAsync(string url, int count = 0)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var response = await _httpClient.GetAsync(url)
                                            .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            // get data string from response
            var data = await response.Content
                                     .ReadAsStringAsync()
                                     .ConfigureAwait(false);

            // serialize to DTO objects
            var container = JsonSerializer.Deserialize<RootContainer>(data, new JsonSerializerOptions
            {
                Converters = { new DateTimeConverter(Constants.DateTimeFormat) }
            });

            if (container == null)
                throw new Exception($"Error getting or parsing posts from {Name}.");

            // order posts by datetime
            var posts = (container.ImportantPosts
                                ?? container.TimelinePosts
                                ?? Enumerable.Empty<ArticlePostDTO>()).OrderByDescending(post => post.Created).ToList();
            
            return posts.Count > 0 && count > 0
                ? posts.Take(count).ToList()
                : posts;
        }
    }
}
