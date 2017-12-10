using System.Threading.Tasks;

namespace Web.Engine.Services
{
    public interface IFileStorage
    {
        Task<string> Save(byte[] buffer, string accessKey);
        Task Delete(string path);
        Task TryDelete(string path);
        FileMeta Open(string path, string accessKey);
    }
}
