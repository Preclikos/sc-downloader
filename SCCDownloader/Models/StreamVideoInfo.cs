using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamVideoInfo
    {
        [JsonPropertyName("width")]
        public int Width;

        [JsonPropertyName("height")]
        public int Height;

        [JsonPropertyName("codec")]
        public String Codec;
    }
}
