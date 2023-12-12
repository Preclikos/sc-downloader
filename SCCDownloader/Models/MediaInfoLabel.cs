using System.Text.Json.Serialization;

namespace SCCDownloader.Models
{
    public class MediaInfoLabel
    {
        [JsonPropertyName("originaltitle")]
        public String OriginalTitle { get; set; }

        [JsonPropertyName("imdb")]
        public String imdb { get; set; }

        [JsonPropertyName("csfd")]
        public String csfd { get; set; }

        [JsonPropertyName("tmdb")]
        public String tmdb { get; set; }
    }
}
