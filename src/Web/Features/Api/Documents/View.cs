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
    public class View
    {
        public class Query : IRequest<Result>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Id)
                    .NotNull()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Read);
            }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IFileStorage _fileStorage;

            public QueryHandler(ApplicationDbContext db, IFileStorage fileStorage)
            {
                _db = db;
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                var revision = await _db.Revisions
                    .Include(r => r.DataFile)
                    .Where(r => r.DocumentId == request.Id.Value)
                    .Where(r => r.EndDate == null)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var file = _fileStorage
                    .Open(revision.DataFile.Path, revision.DataFile.Key, revision.DataFile.IV);

                if (revision == null)
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