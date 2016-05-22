using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Engine.Extensions;
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
            public QueryValidator()
            {
                RuleFor(m => m.Id).NotNull();
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<Result> Handle(Query message)
            {
                const DataProtectionScope dataProtectionScope = DataProtectionScope.LocalMachine;

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
                    FileContents = File
                        .ReadAllBytes(file.ThumbnailPath)
                        .Unprotect(fileKey, dataProtectionScope)
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