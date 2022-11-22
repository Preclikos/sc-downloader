using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamAudioInfo
    {
        [JsonPropertyName("language")]
        public String Language;
        [JsonPropertyName("codec")]
        public String Codec;
        [JsonPropertyName("channels")]
        public int Channels;
    }
}
