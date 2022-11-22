using System.Text.Json.Serialization;

namespace SCCDownloader.MediaInfoModel
{
    public class Media
    {
        [JsonPropertyName("track")]
        public Track[] Track { get; set; }
    }
}
