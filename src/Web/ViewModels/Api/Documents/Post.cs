using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using Web.Engine;
using Web.Engine.Extensions;
using Web.Engine.Services.Lucene;
using Web.Engine.Services.Lucene.Models;
using Web.Models;

namespace Web.ViewModels.Api.Documents
{
    public class Post
    {
        public class Command : IAsyncRequest<int?>
        {
            public IFormFile File { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public int? LibraryIds { get; set; }
            public bool GenerateAbstract { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(ApplicationDbContext db)
            {
                RuleFor(m => m.Abstract)
                    .Length(0, 512);
                RuleFor(m => m.File)
                    .NotNull();
                RuleFor(m => m.LibraryIds)
                    .NotNull()
                    .MustAsync((libraryId, cancellationToken) =>
                        db.Libraries.AnyAsync(l => l.Id == libraryId.Value));
                RuleFor(m => m.Title)
                    .NotNull()
                    .Length(1, 60);
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, int?>
        {
            private readonly ApplicationDbContext _db;
            private readonly IIndexer _indexer;
            private readonly IOptions<DRSSettings> _settings;

            public CommandHandler(ApplicationDbContext db, IIndexer indexer, IOptions<DRSSettings> settings)
            {
                _db = db;
                _indexer = indexer;
                _settings = settings;
            }

            public async Task<int?> Handle(Command message)
            {
                const DataProtectionScope dataProtectionScope = DataProtectionScope.LocalMachine;

                var libraries = await _db.Libraries
                    .Where(l => l.Id == message.LibraryIds.Value)
                    .ToArrayAsync();

                var extension = Path.GetExtension(message.File.FileName() ?? "")
                    .ToLowerInvariant();

                var documentKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));

                var document = new Document
                {
                    Abstract = message.Abstract,
                    CreatedOn = DateTimeOffset.Now,
                    Extension = extension,
                    FileSize = message.File.Length,
                    ModifiedOn = DateTimeOffset.Now,
                    ThumbnailPath = "",
                    Title = message.Title,
                    PageCount = 0,
                    Path = "",
                    Key = Convert.ToBase64String(documentKey.Protect(null, dataProtectionScope))
                };

                _db.Documents.Add(document);

                // add the document to the selected libraries

                document.Libraries.AddRange(libraries.Select(l => new LibraryDocument
                {
                    Library = l
                }));

                using (var trans = await _db.Database.BeginTransactionAsync())
                {
                    await _db.SaveChangesAsync();

                    // generate the document paths

                    var destPath = GetNewFileName(_settings.Value.DocumentDirectory, document.Id);

                    Debug.Assert(destPath != null);

                    if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    }

                    // write the file to the drive

                    var buffer = message.File.OpenReadStream()
                        .ToByteArray();

                    // get a parser

                    var parser = buffer.Parse(extension);

                    // index in lucene

                    var fileContents = await parser.GetContentAsync();

                    if (message.GenerateAbstract)
                    {
                        document.Abstract = fileContents?.Truncate(512);
                    }

                    var indexSuccessful = _indexer.Index(new Index.Command
                    {
                        Id = document.Id,
                        Contents = fileContents
                    });

                    if (!indexSuccessful)
                    {
                        trans.Rollback();
                        return null;
                    }

                    // update the paths on the record

                    document.Path = destPath;
                    document.ThumbnailPath = $"{destPath}{Constants.ThumbnailExtension}";

                    try
                    {
                        // thumbnail

                        using (var stream = new MemoryStream())
                        {
                            // save the thumbnail to the stream

                            await parser.SaveThumbnailAsync(stream);

                            // encrypt the stream and save it

                            await stream.ToArray()
                                .SaveProtectedAsAsync(document.ThumbnailPath, documentKey, dataProtectionScope);
                        }

                        document.PageCount = await parser.GetNumberOfPagesAsync();

                        // encrypt and save the uploaded file

                        await buffer.SaveProtectedAsAsync(document.Path, documentKey, dataProtectionScope);

                        // save and commit

                        await _db.SaveChangesAsync();
                    }
                    catch
                    {
                        // some clean ups

                        // lucene

                        _indexer.Remove(document.Id.ToString());

                        // thumbnail

                        if (File.Exists(document.ThumbnailPath))
                        {
                            File.Delete(document.ThumbnailPath);
                        }

                        // document

                        if (File.Exists(document.Path))
                        {
                            File.Delete(document.Path);
                        }

                        trans.Rollback();

                        throw;
                    }

                    trans.Commit();

                    // the document that was created

                    return document.Id;
                }
            }

            private static string GetNewFileName(string rootPath, int seed)
            {
                var subFolder1 = ((seed / 1024) / 1024);
                var subFolder2 = ((seed / 1024) - (((seed / 1024) / 1024) * 1024));

                return Path.Combine(rootPath,
                    $@"{subFolder1}\{subFolder2}\{Guid.NewGuid():N}.bin");
            }
        }
    }
}