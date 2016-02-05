using System;
using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.FileParsers
{
    public interface IFileParser : IDisposable
    {
        Task<string> GetContentAsync(int? pageNumber = null);
        Task<int> GetNumberOfPagesAsync();
        Task SaveThumbnailAsync(Stream outputStream, int pageNumber = 1, int dpi = 72);
        long FileLength { get; }
    }
}