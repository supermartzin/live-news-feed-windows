using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DenikNcz.DTO
{
    internal class TagDTO : BaseDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}