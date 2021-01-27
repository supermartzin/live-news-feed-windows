using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class TimelineDTO
    {
        [JsonPropertyName("timeline")]
        public IEnumerable<ArticlePostDTO> Posts { get; set; }
    }
}