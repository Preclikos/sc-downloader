using System.Net;

namespace SCCDownloader
{
    public static class WebUtils
    {

        public static Task DownloadAsync(string requestUri, string filename, IProgress<KeyValuePair<long, long>> progress)
        {
            if (requestUri == null)
                throw new ArgumentNullException("requestUri");

            return DownloadAsync(new Uri(requestUri), filename, progress);
        }

        public static async Task DownloadAsync(Uri requestUri, string filename, IProgress<KeyValuePair<long, long>> progress)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");


            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, requestUri))
                {
                    var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                    using (Stream contentStream = await (response).Content.ReadAsStreamAsync(), stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true))
                    {
                        await CopyToAsync(contentStream, (long)response.Content.Headers.ContentLength, stream, 4096, progress, CancellationToken.None);
                        //await contentStream.CopyToAsync(stream);
                    }
                }
            }
        }

        public static async Task CopyToAsync(
    this Stream source,
    long sourceLength,
    Stream destination,
    int bufferSize,
    IProgress<KeyValuePair<long, long>> progress,
    CancellationToken cancellationToken)
        {
            if (0 == bufferSize)
                bufferSize = 4096;
            var buffer = new byte[bufferSize];
            if (0 > sourceLength && source.CanSeek)
                sourceLength = source.Length - source.Position;
            var totalBytesCopied = 0L;
            if (null != progress)
                progress.Report(new KeyValuePair<long, long>(totalBytesCopied, sourceLength));
            var bytesRead = -1;
            while (0 != bytesRead && !cancellationToken.IsCancellationRequested)
            {
                bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
                if (0 == bytesRead || cancellationToken.IsCancellationRequested)
                    break;
                await destination.WriteAsync(buffer, 0, buffer.Length);
                totalBytesCopied += bytesRead;
                if (null != progress)
                    progress.Report(new KeyValuePair<long, long>(totalBytesCopied, sourceLength));
            }
            if (0 < totalBytesCopied)
                progress.Report(new KeyValuePair<long, long>(totalBytesCopied, sourceLength));
            cancellationToken.ThrowIfCancellationRequested();
        }
    }
}
