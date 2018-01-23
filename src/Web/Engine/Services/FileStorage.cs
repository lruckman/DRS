using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using File = System.IO.File;

namespace Web.Engine.Services
{
    public interface IFileStorage
    {
        Task<(byte[] Key, byte[] IV, string Path)> Save(Stream stream);
        Task<string> Save(Stream stream, byte[] key, byte[] iv);

        Task Delete(string path);
        void TryDelete(params string[] paths);

        FileMeta Open(int fileId);
    }

    public class FileStorage : IFileStorage
    {
        private readonly ApplicationDbContext _db;
        private readonly string _storageRoot;
        private readonly IFileEncryptor _encryptor;
        private readonly IFileDecoder _fileDecoder;

        public FileStorage(IOptions<DRSConfig> config, ApplicationDbContext db, IFileEncryptor encryptor, IFileDecoder fileDecoder)
        {
            if (config.Value == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            _db = db ?? throw new ArgumentNullException(nameof(db));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _fileDecoder = fileDecoder ?? throw new ArgumentNullException(nameof(fileDecoder));
            _storageRoot = config.Value.DocumentPath ?? throw new ArgumentNullException(nameof(config.Value.DocumentPath));
        }

        private async Task<int> GetNextDirectorySeed()
        {
            var max = await _db.Documents.MaxAsync(d => d.Id);

            return ++max;
        }

        private FileMeta Open(string path, byte[] key, byte[] iv)
        {
            var fileContents = DecryptFile(path, key, iv);
            var contentType = MimeTypes.MimeTypeMap.GetMimeType(Path.GetExtension(path));

            return new FileMeta(fileContents, contentType, _fileDecoder.Get(path));
        }

        public FileMeta Open(int fileId) // todo: lets start accessing files by id so we don't need to pass around access infos
        {
            var file = _db.PublishedRevisions
                .AsNoTracking()
                .First(r => r.DocumentId == fileId);

            return Open(file.Path, file.AccessKey, file.IV);
        }

        public async Task<(byte[] Key, byte[] IV, string Path)> Save(Stream stream)
        {
            var (Key, IV) = _encryptor.GenerateKeyAndIv();

            var path = await Save(stream, Key, IV);

            return (Key, IV, path);
        }

        public async Task<string> Save(Stream stream, byte[] key, byte[] iv)
        {
            var path = await GetNewFileName();

            await CreateDirectoryIfNotFound(Path.GetDirectoryName(path))
                .ConfigureAwait(false);

            using (var encrypted = _encryptor.Encrypt(stream, key, iv))
            {
                using (var output = File.Create(path))
                {
                    encrypted.CopyTo(output);
                    encrypted.Dispose();
                }
            }

            return path;
        }

        //public async Task<string> Save(byte[] buffer, string accessKey)
        //{
        //    var path = GetNewFileName(await GetNextDirectorySeed(), _storageRoot);

        //    await CreateDirectoryIfNotFound(Path.GetDirectoryName(path))
        //        .ConfigureAwait(false);

        //    buffer = _encryptor
        //        .Encrypt(buffer, DecryptKey(accessKey));

        //    using (var fileStream = File.Create(path))
        //    {
        //        await fileStream
        //            .WriteAsync(buffer, 0, buffer.Length)
        //            .ConfigureAwait(false);
        //    }

        //    return path;
        //}

        public void TryDelete(params string[] paths)
        {
            foreach (var path in paths)
            {
                try
                {
                    Delete(path)
                        .ConfigureAwait(false);
                }
                catch
                {
                }
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

        private async Task<string> GetNewFileName()
            => GetNewFileName(await GetNextDirectorySeed(), _storageRoot);

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