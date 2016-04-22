using System.IO;
using System.Threading.Tasks;

namespace Web.Engine.Codecs.Decoders
{
    public interface IFile
    {
        Task<string> ContentAsync(int? pageNumber = null);
        
        Task<int> PageCountAsync();
        
        Task ThumbnailAsync(Stream outputStream, int width, int? height = null, int pageNumber = 1);
    }
}