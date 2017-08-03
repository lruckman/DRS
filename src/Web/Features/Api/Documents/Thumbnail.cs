using FluentValidation;
using MediatR;
using System;
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
            private readonly IFileStorage _fileStorage;

            public QueryHandler(IFileStorage fileStorage)
            {
                _fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            }

            public async Task<Result> Handle(Query message)
            {
                var currentRevision = await _fileStorage
                    .OpenThumbnail(message.Id.Value)
                    .ConfigureAwait(false);

                if (currentRevision == null)
                {
                    return null;
                }

                return new Result
                {
                    FileContents = currentRevision.FileContents,
                    ContentType = currentRevision.ContentType
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