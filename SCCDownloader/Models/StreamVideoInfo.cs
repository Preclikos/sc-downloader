using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class StreamVideoInfo
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }

        [JsonPropertyName("codec")]
        public String Codec { get; set; }
    }
}
