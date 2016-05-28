using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MimeTypes;
using Web.Engine.Extensions;
using Web.Engine.Helpers;
using Web.Models;
using File = System.IO.File;

namespace Web.ViewModels.Api.Files
{
    public class View
    {
        public class Query : IAsyncRequest<Result>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IDocumentSecurity _documentSecurity;

            public QueryHandler(ApplicationDbContext db, IDocumentSecurity documentSecurity)
            {
                _db = db;
                _documentSecurity = documentSecurity;
            }

            public async Task<Result> Handle(Query message)
            {
                const DataProtectionScope dataProtectionScope = DataProtectionScope.LocalMachine;

                if (!await _documentSecurity.HasFilePermissionAsync(message.Id.Value, PermissionTypes.Read))
                {
                    return new Result
                    {
                        Status = Result.StatusTypes.FailureUnauthorized
                    };
                }

                var file = await _db.Files
                    .Where(f => f.Id == message.Id.Value)
                    .SingleOrDefaultAsync();

                if (file == null)
                {
                    return null;
                }

                var fileKey = Convert.FromBase64String(file.Key)
                    .Unprotect(null, dataProtectionScope);

                var model = new Result
                {
                    FileContents = File.ReadAllBytes(file.Path)
                        .Unprotect(fileKey, dataProtectionScope),
                    ContentType = MimeTypeMap.GetMimeType(file.Extension)
                };

                return model;
            }
        }

        public class Result
        {
            public enum StatusTypes
            {
                FailureUnauthorized,
                Success
            }

            public StatusTypes Status { get; set; } = StatusTypes.Success;
            public byte[] FileContents { get; set; }
            public string ContentType { get; set; }
        }
    }
}