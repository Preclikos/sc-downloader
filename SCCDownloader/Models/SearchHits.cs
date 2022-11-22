using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class SearchHits
    {

        [JsonPropertyName("_id")]
        public String Id { get; set; }

        [JsonPropertyName("_source")]
        public MediaSource Source { get; set; }
    }
}
