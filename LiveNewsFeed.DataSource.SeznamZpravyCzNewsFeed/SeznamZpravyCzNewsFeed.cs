using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

using LiveNewsFeed.Models;

using LiveNewsFeed.DataSource.Common;
using LiveNewsFeed.DataSource.Common.Utilities;

namespace LiveNewsFeed.DataSource.SeznamZpravyCzNewsFeed
{
    public class SeznamZpravyCzNewsFeed : INewsFeed
    {
        private const string FeedUrl = "https://www.seznamzpravy.cz/sekce/stalo-se";

        private readonly ILogger<SeznamZpravyCzNewsFeed>? _logger;

        public string Name => "Seznam Zprávy";

        public SeznamZpravyCzNewsFeed(ILogger<SeznamZpravyCzNewsFeed>? logger = default)
        {
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
                throw new ArgumentNullException(nameof(after));
            if (after >= before)
                throw new ArgumentException("Before and after dates have invalid relative values.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            try
            {
                var allPosts = new List<NewsArticlePost>();

                string? startId = null;
                var parametersFilled = false;
                var passCount = 0;
                const int maxPassCount = 3;
                while (!parametersFilled)
                {
                    allPosts.AddRange(await DownloadPostsAsync(startId).ConfigureAwait(false));
                    allPosts = allPosts.OrderBy(post => post.PublishTime).ToList();

                    // prevent endless loop when no content available
                    if (allPosts.Count == 0 && passCount == maxPassCount)
                        break;

                    // set ID of the oldest post
                    startId = allPosts.FirstOrDefault()?.Id;

                    // check parameter conditions
                    if (before is not null)
                    {
                        if (allPosts.Any(post => post.PublishTime <= before))
                        {
                            parametersFilled = true;

                            // filter only posts before
                            allPosts = allPosts.Where(post => post.PublishTime <= before).ToList();

                            if (count is not null && allPosts.Count < count)
                            {
                                // need to download more posts
                                parametersFilled = false;
                            }
                        }
                        else
                        {
                            passCount++;
                            continue;
                        }
                    }
                    if (after is not null)
                    {
                        if (allPosts.Any(post => post.PublishTime <= after))
                        {
                            // trim all posts before 'after' parameter
                            allPosts = allPosts.Where(post => post.PublishTime >= after).ToList();
                            parametersFilled = true;
                        }
                        else
                        {
                            passCount++;
                            continue;
                        }
                    }
                    if (count is not null && allPosts.Count >= count)
                    {
                        parametersFilled = true;
                        allPosts = allPosts.OrderByDescending(post => post.PublishTime)
                                           .Take(count.Value)
                                           .ToList();
                    }

                    passCount++;
                }

                return allPosts.OrderByDescending(post => post.PublishTime).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting posts from {Name}: {ex.Message}");

                return new List<NewsArticlePost>();
            }
        }


        private async Task<IList<NewsArticlePost>> DownloadPostsAsync(string? startId)
        {
            var htmlWeb = new HtmlWeb();
            
            var url = FeedUrl;
            if (startId is not null)
                url += $"?timeline--pageItem={startId}";

            var page = await htmlWeb.LoadFromWebAsync(url)
                                    .ConfigureAwait(false);
            
            var tasks = page.DocumentNode
                            .SelectNodes("//ul[contains(@class,'timeline')]/li[contains(@class,'d_hK')]/article[not(contains(@class,'advert'))]//a[contains(@class,'title-link')]")?
                            .Select(node => (node.GetAttributeValue("href", null),
                                             node.SelectSingleNode("./ancestor::article[contains(@class,'g_ei')]")?.GetAttributeValue("data-dot-data", null)))
                            .Where(tuple => tuple.Item1 is not null)
                            .Select(ParseArticlePost)
                            .ToArray() ?? Array.Empty<Task<NewsArticlePost?>>();

            if (tasks.Length == 0)
                return Array.Empty<NewsArticlePost>();

            await Task.WhenAll(tasks).ConfigureAwait(false);

            var posts = new List<NewsArticlePost>();
            foreach (var task in tasks)
            {
                if (task.IsCompleted && task.Result is not null)
                    posts.Add(task.Result);
            }

            return posts;
        }

        private async Task<NewsArticlePost?> ParseArticlePost((string Link, string? DateData) linkPair)
        {
            try
            {
                var htmlWeb = new HtmlWeb();
                var page = await htmlWeb.LoadFromWebAsync(linkPair.Link).ConfigureAwait(false);

                var head = page.DocumentNode.SelectSingleNode("//head");
                var body = page.DocumentNode.SelectSingleNode("//body");
                if (body is null || head is null)
                {
                    _logger?.LogWarning($"Invalid page content from {Name} at '{linkPair.Link}'.");
                    return null;
                }

                var id = ParseId(linkPair.Link);
                var title = body.SelectSingleNode(".//section[@data-dot='tpl-content']//h1").InnerText;
                var content = head.SelectSingleNode("./meta[@name='description']").GetAttributeValue("content", string.Empty);
                var publishTime = ParseTime(HttpUtility.HtmlDecode(linkPair.DateData), body.SelectSingleNode(".//span[@class='mol-formatted-date']")?.InnerText);
                var updateTime = ParseTime(null, body.SelectSingleNode(".//div[contains(@data-dot,'publication__updated')]//span[@class='mol-formatted-date']")?.InnerText);
                var imageUrl = head.SelectSingleNode("./meta[@property='og:image']").GetAttributeValue("content", string.Empty);
                var largeImageUrl = ParseLargeImageUrl(imageUrl);
                var imageDescription = body.SelectSingleNode("//span[contains(@class,'caption-text')]")?.InnerText;
                var tags = body.SelectNodes("//div[contains(@data-dot,'related-tags')]//a")?.Select(node => node.InnerText.Trim());

                return new NewsArticlePost(id,
                                           HttpUtility.HtmlDecode(title),
                                           HttpUtility.HtmlDecode(content),
                                           publishTime,
                                           updateTime,
                                           new Uri(linkPair.Link),
                                           false,
                                           Name,
                                           image: new Image(new Uri(imageUrl), imageDescription, new Uri(largeImageUrl)),
                                           tags: tags is not null ? new HashSet<Tag>(tags.Select(tagName => new Tag(HttpUtility.HtmlDecode(tagName)))) : null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error downloading and parsing News Article Post from {Name} at '{linkPair.Link}': {ex.Message}");

                return null;
            }
        }
        
        private static DateTime ParseTime(string? linkTime, string? articleTime)
        {
            var now = DateTime.Now;

            if (linkTime is not null)
            {
                var match = Regex.Match(linkTime, @"""published"":""([\d\-T:.Z]+?)""");
                if (match.Success)
                    return TypeConverter.ToDateTime(match.Groups[1].Value, DateTime.Now);
            }
            else if (articleTime is not null)
            {
                articleTime = Regex.Replace(articleTime, @"[\s\p{Z}]", "");
                if (articleTime.Contains("."))
                {
                    // "11. 7. 20:30"
                    var split = articleTime.Split('.');
                    if (split.Length == 3)
                    {
                        var timeSplit = split[2].Split(':');
                        return new DateTime(now.Year,
                                            TypeConverter.ToInt(split[1], now.Month),
                                            TypeConverter.ToInt(split[0], now.Day),
                                            TypeConverter.ToInt(timeSplit[0], now.Hour),
                                            TypeConverter.ToInt(timeSplit[1], now.Minute), 0);
                    }
                }
                else if (articleTime.Contains("VČERA"))
                {
                    // "VČERA 20:56"
                    articleTime = Regex.Replace(articleTime, @"[^\d:]", "");
                    var split = articleTime.Split(':');
                    if (split.Length == 2)
                    {
                        var yesterday = now.Subtract(TimeSpan.FromDays(1));
                        return new DateTime(yesterday.Year,
                                            yesterday.Month,
                                            yesterday.Day,
                                            TypeConverter.ToInt(split[0], yesterday.Hour),
                                            TypeConverter.ToInt(split[1], yesterday.Minute), 0);
                    }
                }
                else
                {
                    // "20:50"
                    articleTime = Regex.Replace(articleTime, @"[^\d:]", "");
                    var split = articleTime.Split(':');
                    if (split.Length == 2)
                    {
                        return new DateTime(now.Year,
                                            now.Month,
                                            now.Day,
                                            TypeConverter.ToInt(split[0], now.Hour),
                                            TypeConverter.ToInt(split[1], now.Minute), 0);
                    }
                }
            }

            return now;
        }

        private static string ParseId(string link)
        {
            var match = Regex.Match(link, @".*-(\d+)$");

            return match.Success
                ? match.Groups[1].Value
                : link;
        }

        private static string ParseLargeImageUrl(string imageUrl)
        {
            var index = imageUrl.LastIndexOf('?');

            return index > -1
                ? imageUrl.Substring(0, index)
                : imageUrl;
        }
    }
}
