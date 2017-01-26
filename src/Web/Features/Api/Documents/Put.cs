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
    public class Put
    {
        public class Command : IRequest<Result>
        {
            public int? Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public int[] LibraryIds { get; set; } = { };
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Id)
                    .NotNull()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Modify);

                RuleFor(m => m.Abstract)
                    .Length(0, 512);

                RuleFor(m => m.LibraryIds)
                    .NotEmpty().WithName("Libraries")
                    .HasLibraryPermission(documentSecurity, PermissionTypes.Modify);

                RuleFor(m => m.Title)
                    .NotNull()
                    .Length(1, 60);
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IDocumentSecurity _documentSecurity;

            public CommandHandler(ApplicationDbContext db, IDocumentSecurity documentSecurity)
            {
                _db = db;
                _documentSecurity = documentSecurity;
            }

            public async Task<Result> Handle(Command message)
            {
                var document = await _db.Documents
                    .Include(d => d.Distributions)
                    .SingleOrDefaultAsync(d => d.Id == message.Id.Value);

                if (document == null)
                {
                    return null;
                }

                document.ModifiedOn = DateTimeOffset.Now;
                document.Title = message.Title;
                document.Abstract = message.Abstract;

                // remove deleted libraries

                var deletedLibraryIds = document.Distributions
                    .Select(l => l.DistributionGroupId)
                    .Except(message.LibraryIds)
                    .ToArray();

                document.Distributions.RemoveAll(ld => deletedLibraryIds.Contains(ld.DistributionGroupId));

                // add new libraries

                var newLibraryIds = message.LibraryIds
                    .Except(document.Distributions.Select(l => l.DistributionGroupId))
                    .Intersect(await _documentSecurity.GetUserDistributionGroupIdsAsync(PermissionTypes.Modify))
                    .ToArray();

                document.Distributions.AddRange(newLibraryIds.Select(id => new Distribution { DistributionGroupId = id }));

                await _db.SaveChangesAsync();

                return new Result { DocumentId = document.Id };
            }
        }

        public class Result
        {
            public int DocumentId { get; set; }
        }
    }
}