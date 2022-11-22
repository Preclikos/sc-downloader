using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamAudioInfo
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("language")]
        public String Language { get; set; }

        [JsonPropertyName("codec")]
        public String Codec { get; set; }

        [JsonPropertyName("channels")]
        public int Channels { get; set; }
    }
}
