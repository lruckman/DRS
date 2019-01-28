using System.Drawing;
using System.IO;

namespace Web.Engine.Codecs.Decoders
{
    public interface IDecoder
    {
        string TextContent(Stream stream, int? pageNumber = null);
        int PageCount(Stream stream);
        Bitmap CreateThumbnail(Stream stream, Size size, int pageNumber = 1);
        bool AppliesTo(string extension);
    }
}
