using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using LiveNewsFeed.DataSource.DennikNsk.Converters;
using LiveNewsFeed.DataSource.DennikNsk.DTO;

namespace LiveNewsFeed.DataSource.DennikNsk
{
    internal class SkDennikNApiDownloader
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private readonly HttpClient _httpClient;

        public SkDennikNApiDownloader(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IList<ArticlePostDTO>> DownloadPostsAsync(string url, int count = 0)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            try
            {
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();

                // get data string from response
                var data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                // serialize to DTO objects
                var timeline = JsonSerializer.Deserialize<TimelineDTO>(data, new JsonSerializerOptions
                {
                    Converters =
                    {
                        new DateTimeConverter(DateTimeFormat)
                    }
                });

                // order posts by datetime
                var posts = timeline?.Posts.OrderBy(post => post.Created);
                
                // return requested number of elements
                if (posts != null)
                {
                    return count > 0
                        ? posts.Take(count).ToList()
                        : posts.ToList();
                }

                return Enumerable.Empty<ArticlePostDTO>().ToList();

            }
            catch (HttpRequestException hrEx)
            {
                throw new DownloadException($"Error getting posts from Dennik N: {hrEx.Message}", hrEx);
            }
            catch (JsonException jsonEx)
            {
                throw new DownloadException($"Error parsing received data from Dennik N: {jsonEx.Message}", jsonEx);
            }
            catch (Exception ex)
            {
                throw new DownloadException(ex.Message, ex);
            }
        }
    }
}