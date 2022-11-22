using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class MediaSource
    {
        [JsonPropertyName("info_labels")]
        public MediaInfoLabel InfoLabel { get; set; }
    }
}
