using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.FileParsers
{
    public interface IFileParser
    {
        Task<string> ContentAsync(int? pageNumber = null);
        string Content(int? pageNumber);

        Task<int> NumberOfPagesAsync();
        int NumberOfPages();

        Task SaveThumbnailAsync(Stream outputStream, int pageNumber = 1, int dpi = 72);
        void SaveThumbnail(Stream outputStream, int pageNumber, int dpi);
    }
}