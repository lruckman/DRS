using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Models;
using File = System.IO.File;

namespace Web.Engine.Services
{
    public interface IFileStorage
    {
        Task<string> Save(byte[] buffer, byte[] fileKey);
        Task Delete(string filePath);
        Task TryDelete(string filePath);
    }

    public class FileStorage : IFileStorage
    {
        private readonly DRSConfig _config;
        private readonly ApplicationDbContext _db;
        private readonly IFileEncryptor _encryptor;

        public FileStorage(IOptions<DRSConfig> config, ApplicationDbContext db, IFileEncryptor encryptor)
        {
            _config = config.Value;
            _encryptor = encryptor;
            _db = db;
        }

        private async Task CreateDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return;
            }

            await Task.Factory.StartNew(() => Directory.CreateDirectory(dirPath)).ConfigureAwait(false);
        }

        public async Task<string> Save(byte[] buffer, byte[] fileKey)
        {
            var path = await GetNewFileName(_config.DocumentPath).ConfigureAwait(false);

            await CreateDirectory(Path.GetDirectoryName(path)).ConfigureAwait(false);

            buffer = _encryptor.Encrypt(buffer, fileKey);

            using (var fileStream = File.Create(path))
            {
                await fileStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            }

            return path;
        }

        public async Task TryDelete(string filePath)
        {
            try
            {
                await Delete(filePath).ConfigureAwait(false);
            }
            catch
            {
            }
        }

        public async Task Delete(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return;
            }

            await Task.Factory.StartNew(() => File.Delete(filePath)).ConfigureAwait(false);
        }

        private async Task<string> GetNewFileName(string rootPath)
        {
            var seed = await _db.Documents.MaxAsync(d => d.Id).ConfigureAwait(false);

            var subFolder1 = Math.Ceiling((seed + 1) / 1024m / 1024m / 1024m);
            var subFolder2 = Math.Ceiling(subFolder1 / 1024m / 1024m);
            var subFolder3 = Math.Ceiling(subFolder2 / 1024m);

            return Path.Combine(rootPath,
                $@"{subFolder1}\{subFolder2}\{subFolder3}\{Guid.NewGuid():N}.bin");
        }
    }
}