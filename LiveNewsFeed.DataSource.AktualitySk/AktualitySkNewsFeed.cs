using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.Models;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.DataSource.Common.Converters;
using LiveNewsFeed.DataSource.Common.Utilities;

namespace LiveNewsFeed.DataSource.AktualitySk
{
    public class AktualitySkNewsFeed : INewsFeed
    {
        private const string FeedUrl = "https://www.aktuality.sk/_s/api/online-rychle-spravy/";
        private const int PostsCountPerRequest = 20;

        private readonly ILogger<AktualitySkNewsFeed>? _logger;
        private readonly HttpClient _httpClient;

        public string Name => "Aktuality.sk";

        public AktualitySkNewsFeed(HttpClient httpClient, ILogger<AktualitySkNewsFeed>? logger = default)
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
            if (after is not null && after >= DateTime.Now)
                throw new ArgumentOutOfRangeException(nameof(after));
            if (after >= before)
                throw new ArgumentException("Before and after dates have invalid relative values.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            var offset = 0;
            var parametersFilled = false;
            var allShortPosts = new List<ShortPostContainer>();

            while (!parametersFilled)
            {
                allShortPosts.AddRange(await DownloadShortPostsAsync(offset).ConfigureAwait(false));
                allShortPosts = allShortPosts.OrderBy(container => container.PublishTime).ToList();

                offset += PostsCountPerRequest;

                // check parameter conditions
                if (before is not null)
                {
                    if (allShortPosts.Any(container => container.PublishTime <= before))
                    {
                        parametersFilled = true;

                        // filter only posts before
                        allShortPosts = allShortPosts.Where(container => container.PublishTime <= before).ToList();

                        if (count is not null && allShortPosts.Count < count)
                        {
                            // need to download more posts
                            parametersFilled = false;
                        }
                    }
                    else continue;
                }
                if (after is not null)
                {
                    if (allShortPosts.Any(container => container.PublishTime <= after))
                    {
                        // trim all posts before 'after' parameter
                        allShortPosts = allShortPosts.Where(container => container.PublishTime >= after).ToList();
                        parametersFilled = true;
                    }
                    else continue;
                }
                if (count is not null && allShortPosts.Count >= count)
                {
                    parametersFilled = true;
                    allShortPosts = allShortPosts.OrderByDescending(container => container.PublishTime)
                                                 .Take(count.Value)
                                                 .ToList();
                }
            }

            // download full articles from short posts
            var posts = await DownloadPostsAsync(allShortPosts).ConfigureAwait(false);

            return posts.ToList();
        }

        private async Task<IEnumerable<ShortPostContainer>> DownloadShortPostsAsync(int offset)
        {
            try
            {
                var response = await _httpClient.PostAsync(FeedUrl, new StringContent(
                                                                JsonSerializer.Serialize(new {offset}),
                                                                Encoding.UTF8,
                                                                "application/json")).ConfigureAwait(false);

                // get received data
                var data = await response.Content
                                         .ReadAsStreamAsync()
                                         .ConfigureAwait(false);

                // serialize to DTO objects
                var posts = await JsonSerializer.DeserializeAsync<IEnumerable<ShortPostContainer>>(data, new JsonSerializerOptions
                {
                    Converters = {new DateTimeConverter("yyyy'-'MM'-'dd'T'HH':'mm':'ssK") }
                }).ConfigureAwait(false);

                return posts ?? Enumerable.Empty<ShortPostContainer>();
            }
            catch (HttpRequestException httpEx)
            {
                _logger?.LogError(httpEx, $"Error downloading new posts from {Name}: {httpEx.Message}");

                return Enumerable.Empty<ShortPostContainer>();
            }
            catch (JsonException jsonEx)
            {
                _logger?.LogError(jsonEx, $"Error parsing new posts from {Name}: {jsonEx.Message}");

                return Enumerable.Empty<ShortPostContainer>();
            }
        }

        private async Task<IEnumerable<NewsArticlePost>> DownloadPostsAsync(IEnumerable<ShortPostContainer> shortPosts)
        {
            var tasks = shortPosts.Select(DownloadPostAsync).ToArray();

            // wait until all posts are downloaded and parsed
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // process result
            var posts = new List<NewsArticlePost>();
            foreach (var task in tasks)
            {
                if (task.IsCompleted && task.Result is not null)
                {
                    posts.Add(task.Result);
                }
            }

            return posts;
        }

        private async Task<NewsArticlePost?> DownloadPostAsync(ShortPostContainer? shortPost)
        {
            if (shortPost is null)
                return null;

            try
            {
                var htmlWeb = new HtmlWeb();

                // get full article page
                var page = await htmlWeb.LoadFromWebAsync(shortPost.Link)
                                        .ConfigureAwait(false);

                var head = page.DocumentNode.SelectSingleNode("//head");
                var body = page.DocumentNode.SelectSingleNode("//body");

                var title = head.SelectSingleNode("./meta[@property='og:title']")
                                       .GetAttributeValue("content", string.Empty);
                var extendedContent = body.SelectSingleNode(".//div[@itemprop='articleBody']/p[not(strong)][1]")?.InnerText ?? string.Empty;
                var content = body.SelectSingleNode(".//span[@itemprop='description']")?.InnerText ?? extendedContent;
                var publishTime = TypeConverter.ToDateTime(body.SelectSingleNode(".//meta[@itemprop='datePublished']")
                                                               .GetAttributeValue("content", string.Empty), DateTime.Now);
                var updatedTime = TypeConverter.ToDateTime(body.SelectSingleNode(".//meta[@itemprop='dateModified']")
                                                               .GetAttributeValue("content", string.Empty), DateTime.Now);
                var imageUrl = head.SelectSingleNode("./meta[@property='og:image']")
                                   .GetAttributeValue("content", string.Empty);
                imageUrl = HttpUtility.HtmlDecode(imageUrl);
                var imageTitle = body.SelectSingleNode(".//a[@class='title-wrapper']")?.InnerText;
                var categories = ParseCategories(body.SelectSingleNode(".//div[@class='breadcrumbs ']/a[@class='item']")?.InnerText);

                return new NewsArticlePost(shortPost.Id,
                                           HttpUtility.HtmlDecode(title).Trim(),
                                           HttpUtility.HtmlDecode(content).Trim(),
                                           publishTime,
                                           updatedTime,
                                           new Uri(shortPost.Link),
                                           false,
                                           Name,
                                           extendedContent != string.Empty ? HttpUtility.HtmlDecode(extendedContent).Trim() : null,
                                           new Image(new Uri(imageUrl), imageTitle?.Trim(), new Uri(imageUrl)),
                                           categories: new HashSet<Category>(categories));
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error downloading and parsing post from {Name}: {ex.Message}");

                return null;
            }
        }

        private static IEnumerable<Category> ParseCategories(string? categoriesValue)
        {
            if (categoriesValue is null)
                return Enumerable.Empty<Category>();

            categoriesValue = categoriesValue.Trim();

            var categories = new HashSet<Category>();

            if (categoriesValue.Contains("Slovensko"))
            {
                categories.Add(Category.Local);
            }
            if (categoriesValue.Contains("Regióny"))
            {
                categories.Add(Category.Local);
            }
            if (categoriesValue.Contains("Svet"))
            {
                categories.Add(Category.World);
            }
            if (categoriesValue.Contains("Ekonomika"))
            {
                categories.Add(Category.Economy);
            }
            if (categoriesValue.Contains("Kultúra"))
            {
                categories.Add(Category.Arts);
            }
            if (categoriesValue.Contains("Zdravie"))
            {
                categories.Add(Category.Science);
            }
            if (categoriesValue.Contains("Komentáre"))
            {
                categories.Add(Category.Commentary);
            }
            if (categoriesValue.Contains("Cestovanie"))
            {
                categories.Add(Category.Traveling);
            }

            return categories;
        }
    }
}
