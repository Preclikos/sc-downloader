using System.Text.Json.Serialization;

namespace SCCDownloader.MediaInfoModel
{
    public class Track
    {
        [JsonPropertyName("extra")]
        public TrackExtra Extra { get; set; }
    }
}
