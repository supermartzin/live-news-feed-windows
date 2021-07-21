using System;
using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.AktualitySk
{
    internal class ShortPostContainer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Link { get; set; }

        [JsonPropertyName("time")]
        public DateTime PublishTime { get; set; }
    }
}