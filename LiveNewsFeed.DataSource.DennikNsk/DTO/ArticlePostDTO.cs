using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class ArticlePostDTO : BaseDTO
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("created")]
        public DateTime Created { get; set; }

        [JsonPropertyName("updated")]
        public DateTime Updated { get; set; }

        [JsonPropertyName("content")]
        public ContentDTO Content { get; set; }

        [JsonPropertyName("cat")]
        public IEnumerable<CategoryDTO>? Categories { get; set; }

        [JsonPropertyName("tag")]
        public IEnumerable<TagDTO>? Tags { get; set; }

        [JsonPropertyName("image")]
        public ImageDTO? Image { get; set; }

        [JsonPropertyName("embed")]
        public SocialPostDTO? SocialPost { get; set; }

        [JsonPropertyName("important")]
        public int? ImportantCode { get; set; }
    }
}