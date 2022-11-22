using System.Text.Json.Serialization;

namespace SCCDownloader.MediaInfoModel
{
    public class TrackExtra
    {
        [JsonPropertyName("FileExtension_Invalid")]
        public string FileExtensionInvalid { get; set; }
    }
}
