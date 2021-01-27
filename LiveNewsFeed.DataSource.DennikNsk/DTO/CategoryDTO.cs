using System.Text.Json.Serialization;

namespace LiveNewsFeed.DataSource.DennikNsk.DTO
{
    internal class CategoryDTO : BaseDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}