using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DenikNcz.DTO
{
    internal class RootContainer
    {
        [JsonPropertyName("timeline")]
        public IEnumerable<ArticlePostDTO>? TimelinePosts { get; set; }

        [JsonPropertyName("important")]
        public IEnumerable<ArticlePostDTO>? ImportantPosts { get; set; }
    }
}