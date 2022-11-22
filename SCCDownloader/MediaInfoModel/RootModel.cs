using SCCDownloader.MediaInfoModel;
using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class RootModel
    {

        [JsonPropertyName("media")]
        public Media Media { get; set; }
    }
}
