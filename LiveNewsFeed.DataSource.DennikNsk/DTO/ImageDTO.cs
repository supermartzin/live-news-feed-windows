using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class ImageDTO
    {
        [JsonPropertyName("large")]
        public string? LargeSizeUrl { get; set; }

        [JsonPropertyName("medium")]
        public string NormalSizeUrl { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }
    }
}