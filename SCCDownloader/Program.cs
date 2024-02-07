using CommandLine;
using SCCDownloader;
using SCCDownloader.Models;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using XAct;

namespace SCCDownoader // Note: actual namespace depends on the project name.
{
    public class Options
    {
        [Option('s', "showId", Required = true, HelpText = "ShowID from SCC.")]
        public string showId { get; set; }

        [Option('n', "seasonNumber", Required = true, HelpText = "seasonNumber os show.")]
        public int seasonNumber { get; set; }

        [Option('r', "resolution", Required = true, HelpText = "width of video.")]
        public int videoWidth { get; set; }

        [Option('l', "language", Required = true, HelpText = "language of audio.")]
        public string audioLanguage { get; set; }
    }
    internal class Program
    {
        static bool enableMediaInfoExtensions = true;
        static string DownloadFolder = "Downloads";com

        static async Task<int> Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                   .WithParsedAsync<Options>(async o =>
                   {
                       var episodeStream = new List<EpisodeWithStream>();

                       if (!Directory.Exists(DownloadFolder))
                       {
                           Directory.CreateDirectory(DownloadFolder);
                       }

                       var sc = new StreamCinema();
                       var ws = new WebShare();
                       var mytoken = await ws.GetToken("", "");

                       var seasons = await sc.GetShowSeasons(o.showId);
                       if (seasons.Any())
                       {
                           
                           var selectedSeason = seasons.SingleOrDefault(s => s.Number == o.seasonNumber);
                           if (selectedSeason != null)
                           {
                               var episodes = await sc.GetSeasonEpisodes(selectedSeason.Id);
                               foreach (var episode in episodes)
                               {
                                   var streams = await sc.GetStreams(episode.Id);
                                   episodeStream.Add(new EpisodeWithStream(episode.Number, episode.Id, episode.Name, streams));
                               }
                           }

                           var singleEpisodes = GetEpisodeList(episodeStream);

                           var rightEpisodes = singleEpisodes.Where(s => s.AudioLanguages.Contains(o.audioLanguage) && s.VideoWidth == o.videoWidth);
                           var groupEpisodes = rightEpisodes.GroupBy(s => s.EpisodeId);
                           foreach (var groupEpisode in groupEpisodes)
                           {
                               var selEpisode = groupEpisode.OrderByDescending(s => s.Size).First();
                               var pwdName = selEpisode.StreamName + selEpisode.Ident;
                               var shHash = ComputeSha256Hash(pwdName);
                               var directUrl = await ws.GetLink(mytoken, selEpisode.Ident, shHash);
                               string episodeNumber = selEpisode.Number.ToString();
                               Console.WriteLine(episodeNumber + " - " + directUrl);
                           }
                       }
                   });
            return 0;

        }

        static IEnumerable<VideoStream> GetEpisodesStream(IEnumerable<EpisodeWithStream> episodes)
        {
            return episodes.SelectMany(e => e.Streams);
        }

        static IEnumerable<string> Resolutionselector(IEnumerable<EpisodeWithStream> episodes)
        {
            var resolutions = GetEpisodesStream(episodes).SelectMany(v => v.Video).Select(w => w.Width).Distinct();
            return resolutions.Select(i => i.ToString());
        }

        static IEnumerable<string> Audioselector(IEnumerable<EpisodeWithStream> episodes)
        {
            var audios = GetEpisodesStream(episodes).SelectMany(v => v.Audio).Select(w => w.Language).Distinct();
            return audios.Select(i => i.ToString());
        }

        static IEnumerable<SingleEpisode> GetEpisodeList(IEnumerable<EpisodeWithStream> episodes)
        {
            var singleEpisodes = new List<SingleEpisode>();
            foreach (var episode in episodes)
            {
                foreach (var stream in episode.Streams)
                {
                    var singleEpisode = new SingleEpisode();

                    singleEpisode.Number = episode.Number;
                    singleEpisode.EpisodeId = episode.Id;
                    singleEpisode.EpisodeName = episode.Name;

                    singleEpisode.StreamId = stream.Id;
                    singleEpisode.StreamName = stream.Name;
                    singleEpisode.Ident = stream.Ident;

                    var videoStream = stream.Video.Single();
                    singleEpisode.VideoWidth = videoStream.Width;
                    singleEpisode.VideoHeight = videoStream.Height;
                    singleEpisode.VideoHdr = videoStream.Hdr;


                    singleEpisode.AudioLanguages = string.Join(',', stream.Audio.Select(x => x.Language));
                    singleEpisode.SubtitlesLanguages = string.Join(',', stream.Subtitles.Select(x => x.Language));


                    singleEpisodes.Add(singleEpisode);
                }
            }
            return singleEpisodes;
        }


        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static int GetResolution(StreamVideoInfo stream)
        {
            if (stream.Height <= 480)
            {
                return 480;
            }
            else if (stream.Height <= 720)
            {
                return 720;
            }
            else if (stream.Height <= 1080)
            {
                return 1080;
            }
            else if (stream.Height <= 2160)
            {
                return 2160;
            }
            else if (stream.Height <= 4320)
            {
                return 2160;
            }
            return 0;
        }

        static String GetFileName(Movie movie, VideoStream stream)
        {
            String nameWithoutSpaces = movie.Name.Replace(" ", "_");

            string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?";
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());

            foreach (char c in invalid)
            {
                nameWithoutSpaces = nameWithoutSpaces.Replace(c.ToString(), "");
            }

            var resoution = GetResolution(stream.Video.First());

            var audioLangs = String.Join(",", stream.Audio.Select(s => s.Language).Distinct()).ToUpper();
            var subtitleLangs = String.Join(",", stream.Subtitles.Select(s => s.Language).Distinct()).ToUpper();

            String templateSub = "{0}_({1}|Sub:{2})[{3}].unk";
            String template = "{0}_({1})[{3}].unk";

            String selectedTemplate = subtitleLangs.Any() ? templateSub : template;

            return String.Format(template, nameWithoutSpaces, audioLangs, subtitleLangs, resoution);
        }

        static VideoStream GetIdentForParameters(VideoStream[] streams, String language = "cs", Boolean canBeWithSubtitles = true, Boolean canBeFilterSkiped = false)
        {

            var streamsWithSelectedAudio = streams.Where(w => w.Audio.Any(a => a.Language.ToLower() == language));
            if (streamsWithSelectedAudio.Any())
            {
                if (streamsWithSelectedAudio.Count() == 1)
                {
                    return streamsWithSelectedAudio.Single();
                }
                else if (streamsWithSelectedAudio.Any())
                {
                    var maxResolutionItem = streamsWithSelectedAudio.Select(s => s.Video.First()).MaxBy(m => m.Height);
                    return streams.Single(w => w.Video.Contains(maxResolutionItem));
                }
            }

            if (canBeWithSubtitles)
            {
                var streamsWithSelectedSubtitles = streams.Where(w => w.Subtitles.Any(a => a.Language.ToLower() == language));
                if (streamsWithSelectedSubtitles.Count() == 1)
                {
                    return streamsWithSelectedSubtitles.Single();
                }
                else if (streamsWithSelectedSubtitles.Any())
                {
                    var maxResolutionItem = streamsWithSelectedSubtitles.Select(s => s.Video.First()).MaxBy(m => m.Height);
                    return streams.Single(w => w.Video.Contains(maxResolutionItem));
                }
            }

            if (canBeFilterSkiped)
            {
                var maxResolutionItem = streams.Select(s => s.Video.First()).MaxBy(m => m.Height);
                return streams.SingleOrDefault(w => w.Video.Contains(maxResolutionItem));
            }

            return null;
        }

        static String GetInfoStringFromStream(string name, VideoStream stream)
        {
            var video = stream.Video.First();
            var audioLangs = String.Join(", ", stream.Audio.Select(s => s.Language));
            var subtitleLangs = String.Join(", ", stream.Subtitles.Select(s => s.Language));

            string template = "{0} Video:({1}:{2}x{3}) Audio:({4}) Subtitles:({5})";
            return String.Format(template, name, video.Codec, video.Width, video.Height, audioLangs, subtitleLangs);

        }


        static string StartMediaInfoProcess(String filePath)
        {
            //Setting an instance of ProcessStartInfo class
            // under System.Diagnostic Assembly Reference
            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = "./MediaInfo_CLI_22.09_Windows_x64/MediaInfo.exe",
                Arguments = "./" + filePath + " --Output=JSON",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            //Instead of using the above two line of codes, You
            // can just use the code below:
            // ProcessStartInfo pro = new ProcessStartInfo("cmd.exe");
            //Creating an Instance of the Process Class
            // which will help to execute our Process
            Process proStart = new Process();
            //Setting up the Process Name here which we are
            // going to start from ProcessStartInfo
            proStart.StartInfo = StartInfo;
            //Calling the Start Method of Process class to
            // Invoke our Process viz 'cmd.exe'
            proStart.Start();

            StringBuilder stringBuilder = new StringBuilder();
            while (!proStart.StandardOutput.EndOfStream)
            {
                string line = proStart.StandardOutput.ReadLine();
                // do something with line
                stringBuilder.AppendLine(line);
            }
            var result = JsonSerializer.Deserialize<RootModel>(stringBuilder.ToString());

            if (result != null && result.Media != null && result.Media.Track != null && result.Media.Track.Length > 0)
            {
                var track = result.Media.Track.FirstOrDefault();
                if (track != null && track.Extra != null && track.Extra.FileExtensionInvalid != null)
                {
                    if (!String.IsNullOrEmpty(track.Extra.FileExtensionInvalid))
                    {
                        var splitted = track.Extra.FileExtensionInvalid.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        return splitted.FirstOrDefault() ?? "unk";
                    }
                    else
                    {
                        return "unk";
                    }
                }
                else
                {
                    return "unk";
                }
            }
            else
            {
                return "unk";
            }
        }
    }
}