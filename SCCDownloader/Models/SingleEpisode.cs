using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SCCDownloader.Models
{
    public class SingleEpisode
    {
        // episode info
        public int Number { get; set; }
        public String EpisodeId { get; set; }
        public String EpisodeName { get; set; }

        // stream info
        public String StreamId { get; set; }
        public String StreamName { get; set; }
        public String Media { get; set; }
        public String Provider { get; set; }
        public String Ident { get; set; }
        public long Size { get; set; }
//        public StreamVideoInfo[] Video { get; set; }
        public int VideoWidth { get; set; }
        public string VideoHdr { get; set; }
        public int VideoHeight { get; set; }
//        public StreamAudioInfo[] Audio { get; set; }
        public string AudioLanguages  { get; set; }
//        public StreamSubtitleInfo[] Subtitles { get; set; }
        public string SubtitlesLanguages { get; set; }


    }
}
