using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class SearchResponse
    {
        [JsonPropertyName("took")]
        public int Took { get; set; }

        [JsonPropertyName("timed_out")]
        public bool TimeOut { get; set; }

        [JsonPropertyName("hits")]
        public RootHits Hits { get; set; }
    }
}
