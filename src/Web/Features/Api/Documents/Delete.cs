using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Engine.Helpers;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public static class Delete
    {
        public class Command : IRequest<Result>
        {
            public int[] Ids { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Ids)
                    .NotNull()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Delete);
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
            }

            public async Task<Result> Handle(Command message)
            {
                var documents = await _db.PublishedRevisions
                    .Where(d => message.Ids.Contains(d.DocumentId))
                    .ToArrayAsync()
                    .ConfigureAwait(false);

                foreach(var document in documents)
                {
                    document.Status = (int)StatusTypes.Deleted;
                    document.EndDate = DateTimeOffset.Now;
                    //todo: capture users name??
                }

                await _db.SaveChangesAsync()
                    .ConfigureAwait(false);

                return new Result { DocumentIds = message.Ids };
            }
        }

        public class Result
        {
            public int[] DocumentIds { get; set; }
        }
    }
}