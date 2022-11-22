using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class RootHits
    {
        [JsonPropertyName("total")]
        public HitsTotal Total { get; set; }

        [JsonPropertyName("hits")]
        public SearchHits[] Hits { get; set; }
    }
}
