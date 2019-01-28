using System;
using System.Drawing;
using System.IO;
using Web.Engine.Codecs.Decoders;
using Web.Engine.Extensions;

namespace Web.Engine.Services
{
    public interface IFileMeta : IDisposable
    {
        int PageCount();
        string Abstract();
        string Content();
        long Length { get; }
        Bitmap CreateThumbnail(Size dimensions, int? pageNumber);
        Stream FileStream { get; }
    }

    public class FileMeta : IFileMeta
    {
        private readonly IDecoder _decoder;

        private int _pageCount;
        private string _content;

        private readonly Func<Stream> _streamCreator;
        private Stream _fileStream;

        public Stream FileStream
        {
            get
            {
                _fileStream?.Dispose();

                return (_fileStream = _streamCreator());
            }
        }

        public FileMeta(Func<Stream> streamCreator, string contentType, IDecoder decoder)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _streamCreator = streamCreator ?? throw new ArgumentNullException(nameof(streamCreator));
            _fileStream = null;
        }

        public int PageCount() => _pageCount == 0 ? _pageCount = _decoder.PageCount(FileStream) : _pageCount;

        public string Abstract() => Content()?.NormalizeLineEndings()?.Truncate(512);

        public string Content() => string.IsNullOrWhiteSpace(_content) ? _content = _decoder.TextContent(FileStream) : _content;

        public string ContentType { get; }

        public Bitmap CreateThumbnail(Size dimensions, int? pageNumber) => _decoder.CreateThumbnail(FileStream, dimensions, pageNumber ?? 1);

        public long Length => FileStream.Length;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _fileStream?.Dispose();
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
