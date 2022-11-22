using SCCDownloader;
using SCCDownloader.Models;
using System.Net;
using XAct;

namespace SCCDownoader // Note: actual namespace depends on the project name.
{
    internal class Program
    {

        WebClient wc = new WebClient();


        static void Main(string[] args)
        {
            MainAsync().Wait();

        }

        static async Task MainAsync()
        {
            var sc = new StreamCinema();
            var ws = new WebShare();

            Console.Write("Zadej pozadovany rok: ");

            var yearText = Console.ReadLine();
            var year = Int32.Parse(yearText);

            var movies = await sc.GetMovieList(year);
            if (movies.Any())
            {

                Console.Write("Zadej jmeno: ");
                var userName = Console.ReadLine();
                Console.Write("Zadej heslo: ");
                var password = Console.ReadLine();

                if (!String.IsNullOrEmpty(userName) || !String.IsNullOrEmpty(password))
                {
                    var wsToken = await ws.GetToken(userName, password);
                    if (!String.IsNullOrEmpty(wsToken))
                    {
                        foreach (var movie in movies)
                        {
                            var streams = await sc.GetStreams(movie.Id);
                            if (streams.Length > 0)
                            {
                                var streamIdent = GetIdentForParameters(streams);
                                if (streamIdent != null && !String.IsNullOrEmpty(movie.Name))
                                {
                                    Console.WriteLine(GetInfoStringFromStream(movie.Name, streamIdent));
                                    var fileName = GetFileName(movie, streamIdent);

                                    if (!File.Exists(fileName))
                                    {
                                        var link = await ws.GetLink(wsToken, streamIdent.Ident);

                                        var progress = new ProgressBar();
                                        await WebUtils.DownloadAsync(link, fileName, progress);

                                        Console.WriteLine("Completed: " + fileName);
                                    }
                                    else
                                    {
                                        Console.WriteLine("File exist skipping: " + fileName);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("No audio or subtitles exist skip: " + movie.Name);
                                }
                            }
                            else
                            {
                                Console.WriteLine("No streams: " + movie.Name);
                            }
                        }
                    }
                }

            }

            Console.WriteLine("Alles dones to je dat coooo!!!!!");
            Console.ReadLine();
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
    }
}