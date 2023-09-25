using SCCDownloader.Models;

namespace SCCDownloader
{
    public class EpisodeWithStream
    {
        public EpisodeWithStream(int number, string id, string name, IEnumerable<VideoStream> videoStreams)
        {
            Number = number;
            Id = id;
            Name = name;
            VideoStreams = videoStreams;
        }

        public int Number { get; set; }
        public String Id { get; set; }
        public String Name { get; set; }

        public IEnumerable<VideoStream> VideoStreams { get; set; } = new List<VideoStream>();
    }
}
