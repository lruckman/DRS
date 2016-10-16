using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Web.Engine;
using Web.Engine.Extensions;
using Web.Engine.Services;
using Web.Models;
using File = Web.Models.File;

namespace Web.Features.Api.Documents
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
            private readonly IUserContext _userContext;
            private readonly IFileMover _mover;
            private readonly IFileDecoder _decoder;
            private readonly IFileEncryptor _encryptor;

            public CommandHandler(ApplicationDbContext db, IUserContext userContext,
                IFileMover mover, IFileDecoder decoder, IFileEncryptor fileEncryptor)
            {
                _db = db;
                _userContext = userContext;
                _decoder = decoder;
                _encryptor = fileEncryptor;
                _mover = mover;
            }

            public async Task<Result> Handle(Command message)
            {
                var fileKey = Encoding.UTF8
                    .GetBytes(Guid.NewGuid().ToString("N"));

                // create and add the document

                var document = new Document
                {
                    CreatedByUserId = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now,
                    ModifiedOn = DateTimeOffset.Now,
                    Status = StatusTypes.Active,
                    Title = message.File.FileName
                };

                _db.Documents.Add(document);

                // create and add the file

                var file = new File
                {
                    CreatedByUserId = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now,
                    Extension = Path.GetExtension(message.File.FileName ?? "")
                        .ToLowerInvariant(),
                    Key = _encryptor
                        .Encrypt(fileKey, null)
                        .ToBase64String(),
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

                // get a parser

                var fileInfo = _decoder
                    .Decode(message.File.FileName, message.File
                    .OpenReadStream()
                    .ToByteArray());

                document.Abstract = fileInfo.Abstract;

                document.Content = new DocumentContent
                {
                    Content = fileInfo.Content
                };

                try
                {
                    var thumbnail = fileInfo.CreateThumbnail(new Size(600, 600), 1);

                    file.ThumbnailPath = await _mover.Move(thumbnail, fileKey);
                    file.PageCount = fileInfo.PageCount;
                    file.Path = await _mover.Move(fileInfo.Buffer, fileKey);

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

                    throw;
                }

                // the document that was created

                return new Result {DocumentId = document.Id};
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