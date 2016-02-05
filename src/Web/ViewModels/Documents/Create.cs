using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNet.Http;
using Microsoft.Data.Entity;
using Web.Engine;
using Web.Engine.Extensions;
using Web.Engine.Services.Lucene;
using Web.Models;

namespace Web.ViewModels.Documents
{
    public class Create
    {
        public class Query : IAsyncRequest<Command>
        {
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Command>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Command> Handle(Query message)
            {
                var libraries = await _db.Libraries
                    .ToArrayAsync();

                // todo: list here?

                return new Command();
            }
        }

        public class Command : IAsyncRequest<int?>
        {
            public IFormFile File { get; set; }
            public int? LibraryId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(ApplicationDbContext db)
            {
                RuleFor(m => m.File)
                    .NotNull();
                RuleFor(m => m.LibraryId)
                    .NotNull()
                    .MustAsync(async libraryId => await db.Libraries.AnyAsync(l => l.Id == libraryId.Value));
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, int?>
        {
            private readonly ApplicationDbContext _db;
            private readonly IIndexer _indexer;

            public CommandHandler(ApplicationDbContext db, IIndexer indexer)
            {
                _db = db;
                _indexer = indexer;
            }

            public async Task<int?> Handle(Command message)
            {
                const DataProtectionScope dataProtectionScope = DataProtectionScope.LocalMachine;

                var library = await _db.Libraries
                    .SingleAsync(l => l.Id == message.LibraryId.Value);

                var extension = Path.GetExtension(message.File.FileName() ?? "")
                    .ToLowerInvariant();

                using (var trans = await _db.Database.BeginTransactionAsync())
                {
                    var documentKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));

                    var document = new Document
                    {
                        CreatedOn = DateTimeOffset.Now,
                        Extension = extension,
                        FileSize = message.File.Length,
                        ModifiedOn = DateTimeOffset.Now,
                        ThumbnailPath = "",
                        PageCount = 0,
                        Path = "",
                        Key = Convert.ToBase64String(documentKey.Protect(null, dataProtectionScope))
                    };

                    _db.Documents.Add(document);

                    document.Libraries.Add(new LibraryDocument {Library = library});

                    await _db.SaveChangesAsync();

                    // generate the document paths

                    var destPath = GetNewFileName(Startup.Configuration["DRS.DocumentDirectory"],
                        document.Id);

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

                    var indexSuccessful = _indexer.Index(new Engine.Services.Lucene.Models.Index.Command
                    {
                        Id = document.Id,
                        Contents = await parser.GetContentAsync()
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
                var subFolder1 = ((seed/1024)/1024);
                var subFolder2 = ((seed/1024) - (((seed/1024)/1024)*1024));

                return Path.Combine(rootPath,
                    $@"{subFolder1}\{subFolder2}\{Guid.NewGuid():N}.bin");
            }
        }
    }
}