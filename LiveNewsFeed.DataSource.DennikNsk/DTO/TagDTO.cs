using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class TagDTO : BaseDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}