using SCCDownloader.Models;
using System.Net.Http.Json;

namespace SCCDownloader
{
    public class StreamCinema
    {
        static int Limit = 100;

        static protected HttpClient httpClient;
        static readonly String BaseUrl = "https://plugin.sc2.zone";
        static String YearUrl = "/api/media/filter/v2/year?value={0}&order=desc&sort=year&type=movie&size=" + Limit + "&access_token=th2tdy0no8v1zoh1fs59";
        static String YearFromUrl = "/api/media/filter/v2/year?value={0}&order=desc&sort=year&type=movie&size=" + Limit + "&access_token=th2tdy0no8v1zoh1fs59&from={1}";
        
        //static String SearchUrl = "/api/media/filter/v2/search?access_token=th2tdy0no8v1zoh1fs59&order=desc&sort=score&type=movie";
        static String StreamUrl = "/api/media/{0}/streams?access_token=th2tdy0no8v1zoh1fs59";

        public StreamCinema()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<IEnumerable<Movie>> GetMovieList(int year)
        {
            var pathFilter = String.Format(YearUrl, year);
            var response = await httpClient.GetAsync(pathFilter);
            SearchResponse result = await response.Content.ReadFromJsonAsync<SearchResponse>();

            int totalPages = (int)Math.Ceiling((Decimal)result.Hits.Total.Value / Limit);

            Console.WriteLine("Total pages: " + totalPages);

            var movieList = new List<Movie>();

            movieList.AddRange(GetMovieList(result));


            for (int i = 1; i < totalPages; i++)
            {
                pathFilter = String.Format(YearFromUrl, year, i * Limit);
                response = await httpClient.GetAsync(pathFilter);
                result = await response.Content.ReadFromJsonAsync<SearchResponse>();
                movieList.AddRange(GetMovieList(result));
            }

            Console.WriteLine("Info received for: " + movieList.Count);

            return movieList;
        }


        public IEnumerable<Movie> GetMovieList(SearchResponse searchResponse)
        {
            var movieList = new List<Movie>();

            foreach (var movie in searchResponse.Hits.Hits)
            {
                movieList.Add(
                    new Movie
                    {
                        Id = movie.Id,
                        Name = movie.Source.InfoLabel.OriginalTitle
                    }
                    );
            }

            return movieList;
        }

        public async Task<String> GetStreams(string id)
        {
            var pathStream = String.Format(StreamUrl, id);
            var response = await httpClient.GetAsync(pathStream);
            VideoStream[] result = await response.Content.ReadFromJsonAsync<VideoStream[]>();

            return result.First().Ident;
        }
    }
}
