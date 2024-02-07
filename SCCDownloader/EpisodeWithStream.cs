using SCCDownloader.Models;

namespace SCCDownloader
{
    public class EpisodeWithStream
    {
        public EpisodeWithStream(int number, string id, string name, IEnumerable<VideoStream> streams)
        {
            Number = number;
            Id = id;
            Name = name;
            Streams = streams;
        }

        public int Number { get; set; }
        public String Id { get; set; }
        public String Name { get; set; }

        public IEnumerable<VideoStream> Streams { get; set; } = new List<VideoStream>();
    }
}
