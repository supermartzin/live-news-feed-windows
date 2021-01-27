using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class SocialPostDTO
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html")]
        public string EmbedCode { get; set; }
    }
}