using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.Codecs.Decoders
{
    public interface IFile
    {
        Task<string> ContentAsync(int? pageNumber = null);
        string Content(int? pageNumber);

        Task<int> PageCountAsync();
        int PageCount();

        Task ThumbnailAsync(Stream outputStream, int pageNumber = 1, int dpi = 72);
        void Thumbnail(Stream outputStream, int pageNumber, int dpi);
    }
}