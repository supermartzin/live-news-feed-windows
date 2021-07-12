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

namespace LiveNewsFeed.DataSource.AktualneCz
{
    public class AktualneCzNewsFeed : INewsFeed
    {
        private const string FeedUrl = "https://www.aktualne.cz/prave-se-stalo/";
        private const int PostsCountPerPage = 20;

        private readonly ILogger<AktualneCzNewsFeed>? _logger;

        public string Name => "Aktuálně.cz";

        public AktualneCzNewsFeed(ILogger<AktualneCzNewsFeed>? logger = default)
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
                throw new ArgumentOutOfRangeException(nameof(after));
            if (after is not null && before is null && after >= before)
                throw new ArgumentException("Before and after dates have invalid relative values.");
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            try
            {
                var allPosts = new List<NewsArticlePost>();

                var currentPage = 0;
                var parametersFilled = false;
                while (!parametersFilled)
                {
                    allPosts.AddRange(await DownloadPosts(currentPage).ConfigureAwait(false));
                    allPosts = allPosts.OrderBy(post => post.PublishTime).ToList();

                    currentPage++;
                    
                    // check parameter conditions
                    if (before is not null)
                    {
                        var oldestPost = allPosts.FirstOrDefault();
                        if (oldestPost is not null && oldestPost.PublishTime < before)
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
                        else continue;
                    }
                    if (after is not null)
                    {
                        var oldestPost = allPosts.FirstOrDefault();
                        if (oldestPost is not null && oldestPost.PublishTime <= after)
                        {
                            // trim all posts before 'after' parameter
                            allPosts = allPosts.Where(post => post.PublishTime >= after).ToList();
                            parametersFilled = true;
                        }
                    }
                    if (count is not null && allPosts.Count >= count)
                    {
                        allPosts = allPosts.Take(count.Value).ToList();
                        parametersFilled = true;
                    }
                }
                
                return allPosts.OrderByDescending(post => post.PublishTime).ToList();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error getting posts from {Name}: {ex.Message}");

                return new List<NewsArticlePost>();
            }
        }


        private async Task<IList<NewsArticlePost>> DownloadPosts(int pageNumber)
        {
            var htmlWeb = new HtmlWeb();
            var url = $"{FeedUrl}?offset={pageNumber * PostsCountPerPage}";
            
            var page = await htmlWeb.LoadFromWebAsync(url)
                                    .ConfigureAwait(false);

            var posts = new List<NewsArticlePost>();
            posts.AddRange(await ParseShortArticles(page.DocumentNode
                                                        .SelectNodes("//div[@class='timeline']/div[not(contains(@class,'advert')) and contains(@id,'articleshort')]"), url).ConfigureAwait(false));
            posts.AddRange(await ParseLinkPosts(page.DocumentNode
                                                    .SelectNodes("//div[@class='timeline']/div[not(contains(@class,'advert')) and not(contains(@id,'articleshort'))]")).ConfigureAwait(false));
            
            return posts;
        }

        private Task<IList<NewsArticlePost>> ParseShortArticles(HtmlNodeCollection? nodes, string rootUrl)
        {
            return Task.FromResult<IList<NewsArticlePost>>(nodes is not null
                                                            ? nodes.Select(ParseShortArticlePost).ToList()
                                                            : new List<NewsArticlePost>());

            NewsArticlePost ParseShortArticlePost(HtmlNode node)
            {
                string id = node.GetAttributeValue("id", string.Empty);
                string title = string.Empty;
                string content = string.Empty;
                string? extendedContent = null;

                var textNodes = node.SelectNodes(".//p");
                if (textNodes is not null)
                {
                    title = node.SelectSingleNode(".//h3").InnerText.Trim();
                    content = textNodes[0].InnerText.Trim();
                    if (textNodes.Count > 1)
                        extendedContent = GetExtendedContent(textNodes);
                }
                else
                {
                    content = node.SelectSingleNode(".//h3").InnerText;
                }

                var publishTime = ParsePublishTime(node.SelectSingleNode("./div[@class='timeline__label']").InnerText);

                return new NewsArticlePost(id.Replace("articleshort-", ""),
                                           HttpUtility.HtmlDecode(title),
                                           HttpUtility.HtmlDecode(content),
                                           publishTime,
                                           publishTime,
                                           new Uri($"{rootUrl}#{id}"),
                                           false,
                                           Name,
                                           HttpUtility.HtmlDecode(extendedContent));
            }

            static string GetExtendedContent(HtmlNodeCollection textNodes)
            {
                string text = string.Empty;
                
                for (var i = 1; i < textNodes.Count; i++)
                {
                    text += textNodes[i].InnerText.Trim() + Environment.NewLine + Environment.NewLine;
                }

                return text.Trim();
            }

            static DateTime ParsePublishTime(string value)
            {
                var match = Regex.Match(value, @"před\s+(\d)");
                if (!match.Success)
                    return DateTime.Now;

                var number = TypeConverter.ToInt(match.Groups[1].Value);

                if (value.Contains("minut"))
                {
                    return DateTime.Now.Subtract(TimeSpan.FromMinutes(number));
                }
                if (value.Contains("hodin"))
                {
                    return DateTime.Now.Subtract(TimeSpan.FromHours(number));
                }

                return DateTime.Now;
            }
        }

        private async Task<IList<NewsArticlePost>> ParseLinkPosts(HtmlNodeCollection? nodes)
        {
            if (nodes is null)
                return new List<NewsArticlePost>();

            var links = nodes.Select(node => node.SelectSingleNode("./a[1]")?
                                                 .GetAttributeValue("href", null))
                             .Where(link => link is not null)
                             .Cast<string>();

            var tasks = links.Select(ParseLinkPost).ToList();

            // wait until all posts are parsed
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // process results
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

        private async Task<NewsArticlePost?> ParseLinkPost(string link)
        {
            try
            {
                var htmlWeb = new HtmlWeb();
                var page = await htmlWeb.LoadFromWebAsync(link).ConfigureAwait(false);

                var head = page.DocumentNode.SelectSingleNode("//head");

                var id = ParseId(link);
                var title = head.SelectSingleNode("./meta[@name='twitter:title']").GetAttributeValue("content", string.Empty);
                var content = head.SelectSingleNode("./meta[@property='og:description']").GetAttributeValue("content", string.Empty);
                var time = TypeConverter.ToDateTime(head.SelectSingleNode("./meta[@property='article:modified_time']")
                                                                .GetAttributeValue("content", string.Empty), DateTime.Now);
                var extendedContent = page.DocumentNode.SelectSingleNode("//div[@class='article__perex']")?.InnerText?.Trim();
                var imageUrl = head.SelectSingleNode("./meta[@name='twitter:image']").GetAttributeValue("content", string.Empty);
                var largeImageUrl = head.SelectSingleNode("./meta[@property='og:image']").GetAttributeValue("content", string.Empty);
                var tags = head.SelectSingleNode("./meta[@name='keywords']").GetAttributeValue("content", string.Empty)?.Split(new[]{ ", "}, StringSplitOptions.RemoveEmptyEntries);

                return new NewsArticlePost(id,
                                           HttpUtility.HtmlDecode(title),
                                           HttpUtility.HtmlDecode(content),
                                           time, time,
                                           new Uri(link),
                                           false,
                                           Name,
                                           HttpUtility.HtmlDecode(extendedContent),
                                           new Image(new Uri(imageUrl), null, new Uri(largeImageUrl)),
                                           tags: tags is not null ? new HashSet<Tag>(tags.Select(tag => new Tag(HttpUtility.HtmlDecode(tag)))) : null);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error downloading and parsing link post from {Name}: {ex.Message}");

                return null;
            }

            static string ParseId(string link)
            {
                var match = Regex.Match(link, @"\/r~(.*?)\/");

                return match.Success
                    ? match.Groups[1].Value
                    : link;
            }
        }
    }
}
