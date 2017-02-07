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

                var file = await _db.Files
                    .Where(f => f.DocumentId == message.Id.Value
                            && ((message.V == null && f.EndDate == null) || f.VersionNum == message.V.Value)
                            && f.Status != StatusTypes.Deleted)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (file == null)
                {
                    return null;
                }

                var fileKey = _encryptor
                    .DecryptBase64(file.AccessKey);

                return new Result
                {
                    FileContents = _encryptor
                        .DecryptFile(file.ThumbnailPath, fileKey)
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