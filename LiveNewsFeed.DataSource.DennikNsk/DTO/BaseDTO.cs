using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal abstract class BaseDTO
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
    }
}