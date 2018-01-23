using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine;
using Web.Engine.Services;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public class Create
    {
        public class Command : IRequest<Result>
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

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IUserContext _userContext;
            private readonly IFileStorage _fileStorage;
            private readonly IFileEncryptor _encryptor;

            public CommandHandler(ApplicationDbContext db, IUserContext userContext,
                IFileStorage fileStorage, IFileEncryptor fileEncryptor)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
                _encryptor = fileEncryptor ?? throw new ArgumentNullException(nameof(fileEncryptor));
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                // save the file

                var fileInfo = await _fileStorage
                    .Save(request.File.OpenReadStream())
                    .ConfigureAwait(false);


                // create and add the document

                var document = new Document
                {
                    CreatedBy = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now
                };
               
                var revision = new PublishedRevision
                {
                    CreatedBy = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now,
                    Extension = Path
                        .GetExtension(request.File.FileName ?? "")
                        .ToLowerInvariant(),
                    AccessKey = fileInfo.Key,
                    IV = fileInfo.IV,
                    Path = fileInfo.Path,
                    Size = request.File.Length,
                    Title = request.File.FileName,
                    VersionNum = 0
                };

                document.Revisions.Add(revision);

                _db.Documents.Add(document);

                //todo: add to indexers private library (hardcoded for now)
                // add document to the default library

                document.Distributions.Add(await _db.DistributionGroups
                    .Select(d => new Distribution
                    {
                        DistributionGroup = d
                    })
                    .FirstAsync()
                    .ConfigureAwait(false));

                //var file = new FileMeta(
                //    request.File.OpenReadStream().ToByteArray(),
                //    request.File.FileName);

                var decoder = _fileStorage.Open(revision.Path, revision.AccessKey, revision.IV);

                // metadata

                revision.Abstract = decoder.Content(); //todo: decoder should maintain state so we don't have to pass in document buffer?

                try
                {
                    var thumbnail = decoder.CreateThumbnail(new Size(600, 600), 1);

                    revision.ThumbnailPath = await _fileStorage
                        .Save(thumbnail, fileInfo.Key, fileInfo.IV)
                        .ConfigureAwait(false);

                    revision.PageCount = decoder.PageCount();

                    // save and commit

                    await _db.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                catch
                {
                    // some clean ups

                    _fileStorage
                        .TryDelete(revision.ThumbnailPath, revision.Path);

                    throw;
                }

                // the document that was created

                return new Result { Id = document.Id };
            }
        }

        public class Result
        {
            public enum StatusTypes
            {
                Success
            }

            public StatusTypes Status { get; set; } = StatusTypes.Success;
            public int Id { get; set; }
        }
    }
}