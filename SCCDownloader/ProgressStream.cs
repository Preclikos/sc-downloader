namespace SCCDownloader
{
    public class ProgressReportingStream : Stream
    {
        long totalBytesRead;
        long totalBytesWritten;
        long totalSize;
        private void UpdateRead(long read)
        {
            totalBytesRead += read;
            progress.Report(new KeyValuePair<long, long>(totalBytesRead, totalSize));
            //ReadProgress?.Invoke(this, read, totalBytesRead);
        }
        private void UpdateWritten(long written)
        {
            totalBytesWritten += written;
            progress.Report(new KeyValuePair<long, long>(totalBytesWritten, totalSize));
            // WriteProgress?.Invoke(this, written, totalBytesWritten);
        }

        Stream InnerStream { get; init; }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var result = InnerStream.Read(buffer, offset, count);
            UpdateRead(result);
            return result;
        }
        public override void Write(byte[] buffer, int offset, int count)
        {
            InnerStream.Write(buffer, offset, count);
            UpdateWritten(count);
        }
        public override bool CanRead => InnerStream.CanRead;
        public override bool CanSeek => InnerStream.CanSeek;
        public override bool CanWrite => InnerStream.CanWrite;
        public override long Length => InnerStream.Length;
        public override long Position { get => InnerStream.Position; set => InnerStream.Position = value; }
        public ProgressReportingStream(Stream s, IProgress<KeyValuePair<long, long>> progress, long totalSize)
        {
            this.InnerStream = s;
            this.progress = progress;
            this.totalSize = totalSize;
        }

        IProgress<KeyValuePair<long, long>> progress;

        public override void Flush() => InnerStream.Flush();
        public override long Seek(long offset, SeekOrigin origin) => InnerStream.Seek(offset, origin);
        public override void SetLength(long value) => InnerStream.SetLength(value);
    }
}