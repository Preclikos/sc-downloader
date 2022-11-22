using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class MediaInfoLabel
    {
        [JsonPropertyName("originaltitle")]
        public String OriginalTitle { get; set; }
    }
}
