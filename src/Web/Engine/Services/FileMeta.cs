using System;
using System.Drawing;
using System.IO;
using Web.Engine.Codecs.Decoders;
using Web.Engine.Extensions;

namespace Web.Engine.Services
{
    public interface IFileMeta
    {
        int PageCount();
        string Abstract();
        string Content();
        long Length { get; }
        byte[] CreateThumbnail(Size dimensions, int? pageNumber);
    }

    public class FileMeta : IFileMeta
    {
        private readonly IDecoder _decoder;

        private int _pageCount;
        private string _content;

        private Stream _fileStream;

        private Stream FileStream
        {
            set { _fileStream = value;  }
            get
            {
                _fileStream.Position = 0;
                return _fileStream;
            }
        }

        public FileMeta(Stream stream, string contentType, IDecoder decoder)
        {
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            _decoder = decoder ?? throw new ArgumentNullException(nameof(decoder));
            _fileStream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public int PageCount() => _pageCount == 0 ? _pageCount = _decoder.PageCount(FileStream) : _pageCount;

        public string Abstract() => Content()?.NormalizeLineEndings()?.Truncate(512);

        public string Content() => string.IsNullOrWhiteSpace(_content) ? _content = _decoder.TextContent(FileStream) : _content;

        public string ContentType { get; }

        public byte[] CreateThumbnail(Size dimensions, int? pageNumber) => _decoder.CreateThumbnail(FileStream, dimensions, pageNumber ?? 1);

        public long Length => FileStream.Length;
    }
}
