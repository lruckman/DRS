using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Models;
using File = System.IO.File;

namespace Web.Engine.Services
{
    public interface IFileMover
    {
        Task<string> Move(byte[] buffer, byte[] fileKey);
    }

    public class FileMover : IFileMover
    {
        private readonly DRSConfig _config;
        private readonly ApplicationDbContext _db;
        private readonly IFileEncryptor _encryptor;

        public FileMover(IOptions<DRSConfig> config, ApplicationDbContext db, IFileEncryptor encryptor)
        {
            _config = config.Value;
            _encryptor = encryptor;
            _db = db;
        }

        public async Task<string> Move(byte[] buffer, byte[] fileKey)
        {
            var path = await GetNewFileName(_config.DocumentPath);

            buffer = _encryptor.Encrypt(buffer, fileKey);

            using (var fileStream = File.Create(path))
            {
                await fileStream.WriteAsync(buffer, 0, buffer.Length);
            }

            return path;
        }

        private async Task<string> GetNewFileName(string rootPath)
        {
            var seed = await _db.Documents.MaxAsync(d => d.Id);

            var subFolder1 = Math.Ceiling((seed + 1) / 1024m / 1024m / 1024m);
            var subFolder2 = Math.Ceiling(subFolder1 / 1024m / 1024m);
            var subFolder3 = Math.Ceiling(subFolder2 / 1024m);

            return Path.Combine(rootPath,
                $@"{subFolder1}\{subFolder2}\{subFolder3}\{Guid.NewGuid():N}.bin");
        }
    }
}