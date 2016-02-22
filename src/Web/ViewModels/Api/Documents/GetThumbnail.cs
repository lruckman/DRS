using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Engine.Extensions;
using Web.Models;

namespace Web.ViewModels.Api.Documents
{
    public class GetThumbnail
    {
        public class Query : IAsyncRequest<Result>
        {
            public int? DocumentId { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.DocumentId).NotNull();
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

                var document = await _db.Documents
                    .SingleAsync(d => d.Id == message.DocumentId.Value);

                var documentKey = Convert.FromBase64String(document.Key)
                    .Unprotect(null, dataProtectionScope);

                var model = new Result
                {
                    Thumbnail = File.ReadAllBytes(document.ThumbnailPath)
                        .Unprotect(documentKey, dataProtectionScope)
                };

                return model;
            }
        }

        public class Result
        {
            public byte[] Thumbnail { get; set; }
            public string ContentType => "image/png";
        }
    }
}