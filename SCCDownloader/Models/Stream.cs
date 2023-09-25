using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class VideoStream
    {
        [JsonPropertyName("_id")]
        public String Id { get; set; }
        [JsonPropertyName("name")]
        public String Name { get; set; }
        [JsonPropertyName("media")]
        public String Media { get; set; }
        [JsonPropertyName("provider")]
        public String Provider { get; set; }
        [JsonPropertyName("ident")]
        public String Ident { get; set; }
        [JsonPropertyName("size")]
        public long Size { get; set; }
        
        [JsonPropertyName("video")]
        public StreamVideoInfo[] Video { get; set; }

        [JsonPropertyName("audio")]
        public StreamAudioInfo[] Audio { get; set; }

        [JsonPropertyName("subtitles")]
        public StreamSubtitleInfo[] Subtitles { get; set; }
    }
}
