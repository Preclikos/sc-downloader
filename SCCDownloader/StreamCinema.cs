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
        static String ParentUrl = "/api/media/filter/v2/parent?value={0}&sort=episode&access_token=th2tdy0no8v1zoh1fs59";
        //static String SearchUrl = "/api/media/filter/v2/search?access_token=th2tdy0no8v1zoh1fs59&order=desc&sort=score&type=movie";
        static String StreamUrl = "/api/media/{0}/streams?access_token=th2tdy0no8v1zoh1fs59";

        public StreamCinema()
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUrl);
        }

        public async Task<IEnumerable<Season>> GetShowSeasons(string showId)
        {
            var pathFilter = String.Format(ParentUrl, showId);
            var response = await httpClient.GetAsync(pathFilter);
            SearchResponse result = await response.Content.ReadFromJsonAsync<SearchResponse>();

            var seasonList = new List<Season>();

            foreach(var season in result.Hits.Hits)
            {
                var seasonSeason = new Season()
                {
                    Id = season.Id,
                    Number = (int)season.Source.InfoLabel.Season,
                    Name = season.Source.InfoLabel.OriginalTitle
                };
                seasonList.Add(seasonSeason);
            }

            Console.WriteLine("Info received for: " + seasonList.Count);

            return seasonList;
        }

        public async Task<IEnumerable<Episode>> GetSeasonEpisodes(string seasonId)
        {
            var pathFilter = String.Format(ParentUrl, seasonId);
            var response = await httpClient.GetAsync(pathFilter);
            SearchResponse result = await response.Content.ReadFromJsonAsync<SearchResponse>();

            var episodeList = new List<Episode>();

            foreach (var episode in result.Hits.Hits)
            {
                var episodeEpisode = new Episode()
                {
                    Id = episode.Id,
                    Number = (int)episode.Source.InfoLabel.Episode,
                    Name = episode.Source.InfoLabel.OriginalTitle
                };
                episodeList.Add(episodeEpisode);
            }

            Console.WriteLine("Info received for: " + episodeList.Count);

            return episodeList;
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

        public async Task<Models.Stream[]> GetStreams(string id)
        {
            var pathStream = String.Format(StreamUrl, id);
            var response = await httpClient.GetAsync(pathStream);
            //var sda = response.Content.ReadAsStringAsync();
            return await response.Content.ReadFromJsonAsync<Stream[]>();
        }
    }
}
