using SCCDownloader;
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
            if (!movies.Any())
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

                        var streamIdent = await sc.GetStreams(movies.First().Id);

                        var link = await ws.GetLink(wsToken, streamIdent);

                        var progress = new ProgressBar();
                        await WebUtils.DownloadAsync(link, "Test.mkv", progress);
                    }
                }

            }
            Console.ReadLine();
        }
    }
}