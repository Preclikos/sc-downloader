using System.Net;
using System.Security.Cryptography;
using XAct.Resources;

namespace SCCDownloader
{
    public static class WebUtils
    {

        public static Task DownloadAsync(string requestUri, string filename, IProgress<KeyValuePair<long, long>> progress)
        {
            if (requestUri == null)
            {
                //throw new ArgumentNullException("requestUri");
                 Console.WriteLine("Download URI empty, unable to download");
                return Task.CompletedTask;
            }

            else
            {
                return DownloadAsync(new Uri(requestUri), filename, progress);
            }
        }

        public static async Task DownloadAsync(Uri requestUri, string filename, IProgress<KeyValuePair<long, long>> progress)
        {
            if (filename == null)
                throw new ArgumentNullException("Empty filename");


            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    using (Stream contentStream = await (response).Content.ReadAsStreamAsync(), stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 2048, false))
                    {
                        using (ProgressReportingStream progressStream = new ProgressReportingStream(contentStream, progress, (long)response.Content.Headers.ContentLength))
                        {
                            progressStream.CopyTo(stream);
                        }
                        //await CopyToAsync(contentStream, (long)response.Content.Headers.ContentLength, stream, 2048, progress, CancellationToken.None);
                        //await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }
    }
}
