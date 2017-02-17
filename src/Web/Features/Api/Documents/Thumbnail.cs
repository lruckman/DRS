using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
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
            public int? Id { get; set; }
            public int? V { get; set; }
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
            private readonly IFileEncryptor _encryptor;

            public QueryHandler(ApplicationDbContext db, IFileEncryptor encryptor)
            {
                _db = db;
                _encryptor = encryptor;
            }

            public async Task<Result> Handle(Query message)
            {
                Debug.Assert(message.Id != null);

                var currentRevision = await _db.PublishedRevisions
                    .Where(pr => pr.DocumentId == message.Id.Value && pr.EndDate == null)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (currentRevision == null)
                {
                    return null;
                }

                var fileKey = _encryptor
                    .DecryptBase64(currentRevision.AccessKey);

                return new Result
                {
                    FileContents = _encryptor
                        .DecryptFile(currentRevision.ThumbnailPath, fileKey)
                };
            }
        }

        public class Result
        {
            public byte[] FileContents { get; set; }
            public string ContentType => "image/png";
        }
    }
}