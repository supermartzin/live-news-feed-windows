using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DenikNcz.DTO
{
    internal abstract class BaseDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}