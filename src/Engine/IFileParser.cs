using System;
using System.Threading.Tasks;

namespace Engine
{
    public interface IFileParser : IDisposable
    {
        Task<string> Content(int? pageNumber = null);
        Task<int> NumberOfPages();
        Task<string> Thumbnail(string fileName, int pageNumber = 1, int dpi = 72);
        long FileLength { get; }
    }
}