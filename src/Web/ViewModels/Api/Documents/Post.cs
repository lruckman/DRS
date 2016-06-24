using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Web.Engine;
using Web.Engine.Extensions;
using Web.Models;
using File = Web.Models.File;

namespace Web.ViewModels.Api.Documents
{
    public class Post
    {
        public class Command : IAsyncRequest<Result>
        {
            public IFormFile File { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(m => m.File)
                    .NotNull();
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IOptions<DRSSettings> _settings;
            private readonly IUserContext _userContext;
            private readonly IHostingEnvironment _hostingEnvironment;

            public CommandHandler(ApplicationDbContext db, IOptions<DRSSettings> settings, IUserContext userContext,
                IHostingEnvironment hostingEnvironment)
            {
                _hostingEnvironment = hostingEnvironment;
                _db = db;
                _settings = settings;
                _userContext = userContext;
            }

            public async Task<Result> Handle(Command message)
            {
                const DataProtectionScope dataProtectionScope = DataProtectionScope.LocalMachine;

                var extension = Path.GetExtension(message.File.FileName() ?? "")
                    .ToLowerInvariant();

                var fileKey = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString("N"));

                // create and add the document

                var document = new Document
                {
                    CreatedByUserId = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    Status = StatusTypes.Active,
                    Title = message.File.FileName()
                };

                _db.Documents.Add(document);

                // create and add the file

                var file = new File
                {
                    CreatedByUserId = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now,
                    Extension = extension,
                    Key = Convert.ToBase64String(fileKey.Protect(null, dataProtectionScope)),
                    ModifiedOn = DateTimeOffset.Now,
                    PageCount = 0,
                    Path = "",
                    Size = message.File.Length,
                    Status = StatusTypes.Active,
                    ThumbnailPath = "",
                    VersionNum = 1
                };

                document.Files.Add(file);

                //todo: add to indexers private library (hardcoded for now)
                // add document to the default library

                document.Libraries.Add(await _db.Libraries
                    .Select(l => new LibraryDocument
                    {
                        Library = l
                    })
                    .FirstAsync());

                using (var trans = await _db.Database.BeginTransactionAsync())
                {
                    // commit what we have now so we can use the file id to proceed with the rest

                    await _db.SaveChangesAsync();

                    // generate the document paths

                    var destPath = GetNewFileName(_settings.Value.DocumentDirectory, file.Id);

                    Debug.Assert(destPath != null);

                    if (!Directory.Exists(Path.GetDirectoryName(destPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath));
                    }

                    // write the file to the drive

                    var buffer = message.File.OpenReadStream()
                        .ToByteArray();

                    // get a parser

                    var parser = Engine.Codecs.Decoders.File
                        .Get(extension, buffer, _hostingEnvironment.WebRootPath);

                    // index in lucene

                    var fileContents = await parser.ContentAsync();

                    document.Abstract = fileContents
                        ?.NormalizeLineEndings()
                        ?.Truncate(512);

                    document.Content = new DocumentContent
                    {
                        Content = fileContents
                    };

                    // update the paths on the record

                    file.Path = destPath;
                    file.ThumbnailPath = $"{destPath}{Constants.ThumbnailExtension}";

                    try
                    {
                        // thumbnail

                        using (var stream = new MemoryStream())
                        {
                            // save the thumbnail to the stream

                            await parser.ThumbnailAsync(stream, 600);

                            // encrypt the stream and save it

                            await stream.ToArray()
                                .SaveProtectedAsAsync(file.ThumbnailPath, fileKey, dataProtectionScope);
                        }

                        file.PageCount = await parser.PageCountAsync();

                        // encrypt and save the uploaded file

                        await buffer.SaveProtectedAsAsync(file.Path, fileKey, dataProtectionScope);

                        // save and commit

                        await _db.SaveChangesAsync();
                    }
                    catch
                    {
                        // some clean ups

                        // thumbnail

                        if (System.IO.File.Exists(file.ThumbnailPath))
                        {
                            System.IO.File.Delete(file.ThumbnailPath);
                        }

                        // document

                        if (System.IO.File.Exists(file.Path))
                        {
                            System.IO.File.Delete(file.Path);
                        }

                        trans.Rollback();

                        throw;
                    }

                    trans.Commit();

                    // the document that was created

                    return new Result {DocumentId = document.Id};
                }
            }

            private static string GetNewFileName(string rootPath, int seed)
            {
                var subFolder1 = Math.Ceiling(seed / 1024m / 1024m / 1024m);
                var subFolder2 = Math.Ceiling(subFolder1 / 1024m / 1024m);
                var subFolder3 = Math.Ceiling(subFolder2 / 1024m);

                return Path.Combine(rootPath,
                    $@"{subFolder1}\{subFolder2}\{subFolder3}\{Guid.NewGuid():N}.bin");
            }
        }

        public class Result
        {
            public enum StatusTypes
            {
                Success
            }

            public StatusTypes Status { get; set; } = StatusTypes.Success;
            public int DocumentId { get; set; }
        }
    }
}