using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine.Helpers;
using Web.Engine.Services;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public class Thumbnail
    {
        public class Query : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Id)
                    .NotEmpty()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Read);
            }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly IFileStorage _fileStorage;
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db, IFileStorage fileStorage)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var dataFileId = await _db.Revisions
                    .Where(r => r.DocumentId == request.Id)
                    .Where(r => r.EndDate == null)
                    .Select(r => r.DataFileId)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var file = await _fileStorage
                    .Open(dataFileId, true)
                    .ConfigureAwait(false);

                if (file == null)
                {
                    return null;
                }

                return new Result
                {
                    FileContents = file.FileStream,
                    ContentType = file.ContentType
                };
            }
        }

        public class Result
        {
            public Stream FileContents { get; set; }
            public string ContentType { get; set; }
        }
    }
}