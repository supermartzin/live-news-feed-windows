using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DenikNcz.DTO
{
    internal class ContentDTO
    {
        [JsonPropertyName("main")]
        public string MainText { get; set; }

        [JsonPropertyName("extended")]
        public string? ExtendedText { get; set; }
    }
}