using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Models;
using File = System.IO.File;
using System.Linq;
using MimeTypes;

namespace Web.Engine.Services
{
    public class EncryptedFileStorage : IFileStorage
    {
        private readonly DRSConfig _config;
        private readonly ApplicationDbContext _db;
        private readonly IFileEncryptor _encryptor;

        public EncryptedFileStorage(IOptions<DRSConfig> config, ApplicationDbContext db, IFileEncryptor encryptor)
        {
            _config = config.Value;
            _encryptor = encryptor;
            _db = db;
        }

        public async Task<FileMeta> Open(int documentId, int? versionNum = null)
        {
            var revision = await GetRevision(documentId, versionNum)
                .ConfigureAwait(false);

            return Open(
                revision.Path,
                revision.Extension,
                revision.AccessKey
                );
        }

        public async Task<FileMeta> OpenThumbnail(int documentId, int? versionNum = null)
        {
            var revision = await GetRevision(documentId, versionNum)
                .ConfigureAwait(false);

            return Open(
                revision.ThumbnailPath,
                revision.Extension,
                revision.AccessKey
                );
        }

        public FileMeta Open(string path, string extension, string accessKey)
        {
            var fileContents = DecryptFile(path, accessKey);

            var contentType = MimeTypeMap.GetMimeType(extension);

            return new FileMeta(fileContents, contentType);
        }

        public async Task<string> Save(byte[] buffer, byte[] fileKey)
        {
            var path = await GetNewFileName(_config.DocumentPath)
                .ConfigureAwait(false);

            await CreateDirectoryIfMissing(Path.GetDirectoryName(path))
                .ConfigureAwait(false);

            buffer = _encryptor
                .Encrypt(buffer, fileKey);

            using (var fileStream = File.Create(path))
            {
                await fileStream
                    .WriteAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false);
            }

            return path;
        }

        public async Task TryDelete(string filePath)
        {
            try
            {
                await Delete(filePath)
                    .ConfigureAwait(false);
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

            await Task.Factory
                .StartNew(() => File.Delete(filePath))
                .ConfigureAwait(false);
        }

        private async Task<string> GetNewFileName(string rootPath)
        {
            var seed = await _db.Documents
                .MaxAsync(d => d.Id)
                .ConfigureAwait(false);

            var subFolder1 = Math.Ceiling((seed + 1) / 1024m / 1024m / 1024m);
            var subFolder2 = Math.Ceiling(subFolder1 / 1024m / 1024m);
            var subFolder3 = Math.Ceiling(subFolder2 / 1024m);

            return Path.Combine(rootPath,
                $@"{subFolder1}\{subFolder2}\{subFolder3}\{Guid.NewGuid():N}.bin");
        }

        private Task CreateDirectoryIfMissing(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                return Task.FromResult(0);
            }

            return Task.Factory
                .StartNew(() => Directory.CreateDirectory(dirPath));
        }

        private Task<Revision> GetRevision(int documentId, int? versionNum = null)
        {
            if (versionNum != null)
            {
                return _db.Revisions
                    .FindAsync(documentId, versionNum);
            }

            return _db.Revisions
                    .Where(r => r.DocumentId == documentId)
                    .Where(r => r.EndDate == null)
                    .SingleAsync();
        }

        private byte[] DecryptFile(string path, string accessKey)
        {
            var fileKey = _encryptor
                    .DecryptBase64(accessKey);

            return _encryptor
                .DecryptFile(path, fileKey);
        }
    }
}