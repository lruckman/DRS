using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Engine.Extensions;
using Web.Models;

namespace Web.ViewModels.Documents
{
    public class View
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
                    Document = File.ReadAllBytes(document.Path)
                        .Unprotect(documentKey, dataProtectionScope)
                };

                return model;
            }
        }

        public class Result
        {
            public byte[] Document { get; set; }
            public string ContentType => "application/pdf";
        }
    }
}
