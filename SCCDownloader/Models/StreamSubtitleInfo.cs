using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamSubtitleInfo
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("language")]
        public String Language { get; set; }

        [JsonPropertyName("forced")]
        public Boolean Forced { get; set; }

        [JsonPropertyName("src")]
        public String Src { get; set; }
    }
}
