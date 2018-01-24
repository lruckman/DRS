using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
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

            public CommandHandler(ApplicationDbContext db, IUserContext userContext,
                IFileStorage fileStorage)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                // save the file

                var fileInfo = await _fileStorage
                    .Save(request.File.OpenReadStream())
                    .ConfigureAwait(false);

                // create and add the document

                var dataFile = new DataFile
                {
                    Extension = Path
                        .GetExtension(request.File.FileName ?? "")
                        .ToLowerInvariant(),
                    Key = fileInfo.Key,
                    IV = fileInfo.IV,
                    Path = fileInfo.Path,
                    Size = request.File.Length,
                };

                _db.DataFiles.Add(dataFile);
                
                var document = new Document
                {
                    CreatedBy = _userContext.UserId,
                    CreatedOn = DateTimeOffset.Now
                };

                _db.Documents.Add(document);

                using (var decoder = _fileStorage
                    .Open(dataFile.Path, dataFile.Key, dataFile.IV))
                {

                    var revision = new PublishedRevision
                    {
                        Abstract = decoder.Content(),
                        CreatedBy = _userContext.UserId,
                        CreatedOn = DateTimeOffset.Now,
                        DataFile = dataFile,
                        Title = request.File.FileName,
                        VersionNum = 0
                    };

                    document.Revisions.Add(revision);

                    //todo: add to indexers private library (hardcoded for now)
                    // add document to the default library

                    document.Distributions.Add(await _db.DistributionGroups
                        .Select(d => new Distribution
                        {
                            DistributionGroup = d
                        })
                        .FirstAsync()
                        .ConfigureAwait(false));

                    try
                    {
                        dataFile.PageCount = decoder.PageCount();

                        //var thumbnail = decoder.CreateThumbnail(new Size(600, 600), 1);
                        //await _fileStorage
                        //    .Save(thumbnail, fileInfo.Key, fileInfo.IV)
                        //                .ConfigureAwait(false);



                        // save and commit

                        await _db.SaveChangesAsync()
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // some clean ups

                        _fileStorage
                            .TryDelete(dataFile.ThumbnailPath, dataFile.Path);

                        throw;
                    }
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