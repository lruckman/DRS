using System.Drawing;
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

        public abstract string TextContent(byte[] buffer, int? pageNumber);
        public abstract int PageCount(byte[] buffer);
        public abstract byte[] CreateThumbnail(byte[] buffer, Size size, int pageNumber = 1);

        public virtual bool AppliesTo(string extension) => _supportedFileTypes.Contains(extension);
    }
}