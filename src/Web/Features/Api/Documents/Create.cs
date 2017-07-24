using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Engine;
using Web.Engine.Extensions;
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

        public class CommandHandler : IAsyncRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IUserContext _userContext;
            private readonly IFileStorage _fileStorage;
            private readonly IFileDecoder _decoder;
            private readonly IFileEncryptor _encryptor;

            public CommandHandler(ApplicationDbContext db, IUserContext userContext,
                IFileStorage fileStorage, IFileDecoder decoder, IFileEncryptor fileEncryptor)
            {
                _db = db;
                _userContext = userContext;
                _decoder = decoder;
                _encryptor = fileEncryptor;
                _fileStorage = fileStorage;
            }

            public async Task<Result> Handle(Command message)
            {
                var fileKey = Encoding.UTF8
                    .GetBytes(Guid.NewGuid().ToString("N"));

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
                    Extension = Path.GetExtension(message.File.FileName ?? "")
                        .ToLowerInvariant(),
                    AccessKey = _encryptor
                        .Encrypt(fileKey, null)
                        .ToBase64String(),
                    Size = message.File.Length,
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

                // get a parser

                var fileInfo = _decoder
                    .Decode(message.File.FileName, message.File
                    .OpenReadStream()
                    .ToByteArray());

                // metadata

                revision.Abstract = fileInfo.Abstract;
                revision.Title = message.File.FileName;

                try
                {
                    var thumbnail = fileInfo.CreateThumbnail(new Size(600, 600), 1);

                    revision.ThumbnailPath = await _fileStorage
                        .Save(thumbnail, fileKey)
                        .ConfigureAwait(false);

                    revision.PageCount = fileInfo.PageCount;

                    revision.Path = await _fileStorage
                        .Save(fileInfo.Buffer, fileKey)
                        .ConfigureAwait(false);

                    // save and commit

                    await _db.SaveChangesAsync()
                        .ConfigureAwait(false);
                }
                catch
                {
                    // some clean ups

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                    _fileStorage.TryDelete(revision.ThumbnailPath);
                    _fileStorage.TryDelete(revision.Path);

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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