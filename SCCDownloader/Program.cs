using SCCDownloader;
using SCCDownloader.Models;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;
using XAct;

namespace SCCDownoader // Note: actual namespace depends on the project name.
{
    internal class Program
    {
        static bool enableMediaInfoExtensions = true;

        //static string DownloadFolder = "Downloads"; //replaced by making folders by year 

        static void Main(string[] args)
        {
            MainAsync().Wait();

        }

        static async Task MainAsync()
        {
            Console.Write("Zadej pozadovany rok: ");

            var yearText = Console.ReadLine();
            var year = Int32.Parse(yearText);

            var DownloadFolder = yearText;


            if (!Directory.Exists(DownloadFolder))
            {
                Directory.CreateDirectory(DownloadFolder);
            }

            var sc = new StreamCinema();
            var ws = new WebShare();

            

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

                                    if (!File.Exists(DownloadFolder + "/" + fileName))
                                    {
                                        var link = await ws.GetLink(wsToken, streamIdent.Ident);

                                        var progress = new ProgressBar();
                                        await WebUtils.DownloadAsync(link, DownloadFolder + "/" + fileName, progress);
                                        if (enableMediaInfoExtensions)
                                        {
                                            var extension = StartMediaInfoProcess(DownloadFolder + "/" + fileName);
                                            if (extension != "unk")
                                            {
                                                File.Move(DownloadFolder + "/" + fileName, DownloadFolder + "/" + fileName.Replace(".unk", "." + extension));
                                                File.Create(DownloadFolder + "/" + fileName);
                                                fileName = fileName.Replace(".unk", "." + extension);
                                            }
                                        }
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
            // replace empty names with tmdb and fallback to movieID 
            if (movie.Name.Length == 0) { movie.Name = movie.Id; };
            
            String nameWithoutSpaces = movie.Name.Replace(" ", "_");


            //string illegal = "\"M\"\\a/ry/ h**ad:>> a\\/:*?\"| li*tt|le|| la\"mb.?"; //asi pokus z testování illegal stringu? 

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