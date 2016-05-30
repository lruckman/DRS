using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Engine.Extensions;
using Web.Engine.Helpers;
using Web.Engine.Validation.Custom;
using Web.Models;
using File = System.IO.File;

namespace Web.ViewModels.Api.Files
{
    public class Thumbnail
    {
        public class Query : IAsyncRequest<Result>
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

            private const DataProtectionScope DataProtectionScope =
                System.Security.Cryptography.DataProtectionScope.LocalMachine;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message)
            {
                Debug.Assert(message.Id != null);

                var file = await _db.Files
                    .Where(f => f.Id == message.Id.Value)
                    .SingleOrDefaultAsync();

                if (file == null)
                {
                    return null;
                }

                var fileKey = Convert.FromBase64String(file.Key)
                    .Unprotect(null, DataProtectionScope);

                var model = new Result
                {
                    FileContents = File
                        .ReadAllBytes(file.ThumbnailPath)
                        .Unprotect(fileKey, DataProtectionScope)
                };

                return model;
            }
        }

        public class Result
        {
            public byte[] FileContents { get; set; }
            public string ContentType => "image/png";
        }
    }
}