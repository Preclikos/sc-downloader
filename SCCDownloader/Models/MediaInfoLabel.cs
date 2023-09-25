using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class MediaInfoLabel
    {
        [JsonPropertyName("originaltitle")]
        public String OriginalTitle { get; set; } = String.Empty;
        [JsonPropertyName("episode")]
        public int? Episode { get; set; }
        [JsonPropertyName("season")]
        public int? Season { get; set; }
    }
}
