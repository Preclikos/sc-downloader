using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamSubtitleInfo
    {
        [JsonPropertyName("language")]
        public String Language;

        [JsonPropertyName("forced")]
        public Boolean Forced;
    }
}
