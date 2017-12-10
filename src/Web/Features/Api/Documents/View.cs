using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IFileStorage _fileStorage;

            public QueryHandler(ApplicationDbContext db, IFileStorage fileStorage)
            {
                _db = db;
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Query message)
            {
                var revision = await _db.Revisions
                    .Where(r => r.DocumentId == message.Id.Value)
                    .Where(r => r.EndDate == null)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var file = _fileStorage
                    .Open(revision.Path, revision.AccessKey);

                if (revision == null)
                {
                    return null;
                }

                return new Result
                {
                    FileContents = file.Buffer,
                    ContentType = file.ContentType
                };
            }
        }

        public class Result
        {
            public byte[] FileContents { get; set; }
            public string ContentType { get; set; }
        }
    }
}