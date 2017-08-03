using System.Threading.Tasks;

namespace Web.Engine.Services
{
    public interface IFileStorage
    {
        Task<string> Save(byte[] buffer, byte[] fileKey);
        Task Delete(string filePath);
        Task TryDelete(string filePath);
        FileMeta Open(string path, string extension, string accessKey);
        Task<FileMeta> Open(int documentId, int? versionNum = null);
        Task<FileMeta> OpenThumbnail(int documentId, int? versionNum = null);
    }
}
