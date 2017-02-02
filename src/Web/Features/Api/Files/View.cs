using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MimeTypes;
using System.Linq;
using System.Threading.Tasks;
using Web.Engine.Helpers;
using Web.Engine.Services;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Files
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
                    .HasDocumentFileAccess(documentSecurity, PermissionTypes.Read);
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
                var file = await _db.Files
                    .Where(f => f.Id == message.Id.Value)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (file == null)
                {
                    return null;
                }

                var fileKey = _encryptor
                    .DecryptBase64(file.Key);

                return new Result
                {
                    FileContents = _encryptor
                        .DecryptFile(file.Path, fileKey),
                    ContentType = MimeTypeMap.GetMimeType(file.Extension)
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