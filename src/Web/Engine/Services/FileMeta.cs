using System.Drawing;
using Web.Engine.Codecs.Decoders;
using Web.Engine.Extensions;

namespace Web.Engine.Services
{
    public interface IFileMeta
    {
        int PageCount { get; }
        string Abstract { get; }
        string Content { get; }
        long Length { get; }
        byte[] CreateThumbnail(Size dimensions, int? pageNumber);
        byte[] Buffer { get; }
    }

    public class FileMeta : IFileMeta
    {
        private readonly IDecoder _decoder;

        private int _pageCount;
        private string _content;

        public FileMeta(byte[] buffer, string extension)
        {
            Buffer = buffer;

            ContentType = MimeTypes.MimeTypeMap.GetMimeType(extension);
            _decoder = new FileDecoder().Get(extension);
        }

        public int PageCount => _pageCount == 0 ? _pageCount = _decoder.PageCount(Buffer) : _pageCount;

        public string Abstract => Content?.NormalizeLineEndings()?.Truncate(512);

        public string Content => string.IsNullOrWhiteSpace(_content) ? _content = _decoder.TextContent(Buffer) : _content;

        public string ContentType { get; }

        public byte[] CreateThumbnail(Size dimensions, int? pageNumber) => _decoder.CreateThumbnail(Buffer, dimensions, pageNumber ?? 1);

        public byte[] Buffer { get; }

        public long Length => Buffer.LongLength;
    }
}
