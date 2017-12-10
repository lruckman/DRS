using System.Drawing;

namespace Web.Engine.Codecs.Decoders
{
    public interface IDecoder
    {
        string TextContent(byte[] buffer, int? pageNumber = null);
        int PageCount(byte[] buffer);
        byte[] CreateThumbnail(byte[] buffer, Size size, int pageNumber = 1);
        bool AppliesTo(string extension);
    }
}
