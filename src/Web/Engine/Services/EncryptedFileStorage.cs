using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using File = System.IO.File;

namespace Web.Engine.Services
{
    public class EncryptedFileStorage : IFileStorage
    {
        private readonly string _storageRoot;
        private readonly IFileEncryptor _encryptor;
        private int _directorySeed;

        public EncryptedFileStorage(IOptions<DRSConfig> config, ApplicationDbContext db, IFileEncryptor encryptor)
        {
            if (config.Value == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _storageRoot = config.Value.DocumentPath;

            if (db == null)
            {
                throw new ArgumentNullException(nameof(db));
            }

            _directorySeed = db.Documents
                .Max(d => d.Id);
        }

        public FileMeta Open(string path, string accessKey)
        {
            var fileContents = DecryptFile(path, accessKey);
            var extension = Path.GetExtension(path);

            return new FileMeta(fileContents, extension);
        }

        public async Task<string> Save(byte[] buffer, string accessKey)
        {
            var path = GetNewFileName(++_directorySeed, _storageRoot);

            await CreateDirectoryIfNotFound(Path.GetDirectoryName(path))
                .ConfigureAwait(false);

            buffer = _encryptor
                .Encrypt(buffer, DecryptKey(accessKey));

            using (var fileStream = File.Create(path))
            {
                await fileStream
                    .WriteAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false);
            }

            return path;
        }

        public async Task TryDelete(string path)
        {
            try
            {
                await Delete(path)
                    .ConfigureAwait(false);
            }
            catch
            {
            }
        }

        public async Task Delete(string path)
        {
            if (!File.Exists(path))
            {
                return;
            }

            await Task.Factory
                .StartNew(() => File.Delete(path))
                .ConfigureAwait(false);
        }

        private static string GetNewFileName(int directorySeed, string rootPath)
        {
            var subFolder1 = Math.Floor(directorySeed / 1024m / 1024m / 1024m);
            var subFolder2 = Math.Floor(subFolder1 / 1024m / 1024m);
            var subFolder3 = Math.Floor(subFolder2 / 1024m);

            return Path.Combine(rootPath,
                $@"{subFolder1}\{subFolder2}\{subFolder3}\{Guid.NewGuid():N}.bin");
        }

        private Task CreateDirectoryIfNotFound(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return Task.FromResult(0);
            }

            return Task.Factory
                .StartNew(() => Directory.CreateDirectory(dirPath));
        }

        private byte[] DecryptKey(string accessKey) => _encryptor
                    .DecryptBase64(accessKey);

        private byte[] DecryptFile(string path, string accessKey) => _encryptor
                .DecryptFile(path, DecryptKey(accessKey));
    }
}