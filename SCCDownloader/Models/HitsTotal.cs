using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class HitsTotal
    {

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
