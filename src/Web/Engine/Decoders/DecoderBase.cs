using System;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Web.Engine.Codecs.Decoders
{
    public abstract class DecoderBase : IDecoder
    {
        private readonly string[] _supportedFileTypes;

        protected DecoderBase(string[] supportedFileTypes)
        {
            _supportedFileTypes = supportedFileTypes;
        }

        public abstract string TextContent(Stream stream, int? pageNumber);
        public abstract int PageCount(Stream stream);
        public abstract byte[] CreateThumbnail(Stream stream, Size size, int pageNumber = 1);

        public virtual bool AppliesTo(string extension) => _supportedFileTypes.Contains(extension, StringComparer.InvariantCultureIgnoreCase);
    }
}