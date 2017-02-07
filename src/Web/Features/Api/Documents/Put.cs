using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Engine;
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
            public int[] LibraryIds { get; set; } = { }; //todo: rename
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
            private readonly IUserContext _userContext;

            public CommandHandler(ApplicationDbContext db, IDocumentSecurity documentSecurity, IUserContext userContext)
            {
                _db = db;
                _documentSecurity = documentSecurity;
                _userContext = userContext;
            }

            public async Task<Result> Handle(Command message)
            {
                var file = await _db.Files
                    .Include(f => f.Document)
                    .Include(f => f.Document.Distributions)
                    .Include(f => f.Metadata)
                    .SingleOrDefaultAsync(f => f.DocumentId == message.Id.Value && f.EndDate == null)
                    .ConfigureAwait(false);

                if (file == null)
                {
                    return null;
                }

                var currentMetadata = file.Metadata
                    .Single(m => m.EndDate == null);

                if (currentMetadata.Title != message.Title
                    || currentMetadata.Abstract != message.Abstract)
                {
                    currentMetadata.EndDate = DateTimeOffset.Now;
                    file.Metadata.Add(new Metadata
                    {
                        Abstract = message.Abstract,
                        CreatedBy = _userContext.UserId,
                        CreatedOn = DateTimeOffset.Now,
                        Title = message.Title,
                        VersionNum = ++currentMetadata.VersionNum
                    });
                }

                // remove deleted libraries

                var deletedLibraryIds = file.Document.Distributions
                    .Select(l => l.DistributionGroupId)
                    .Except(message.LibraryIds)
                    .ToArray();

                file.Document.Distributions.RemoveAll(ld => deletedLibraryIds.Contains(ld.DistributionGroupId));

                // add new libraries

                var newLibraryIds = message.LibraryIds
                    .Except(file.Document.Distributions.Select(l => l.DistributionGroupId))
                    .Intersect(await _documentSecurity.GetUserDistributionGroupIdsAsync(PermissionTypes.Modify).ConfigureAwait(false))
                    .ToArray();

                file.Document.Distributions.AddRange(newLibraryIds.Select(id => new Distribution { DistributionGroupId = id }));

                await _db.SaveChangesAsync().ConfigureAwait(false);

                return new Result { DocumentId = file.DocumentId };
            }
        }

        public class Result
        {
            public int DocumentId { get; set; }
        }
    }
}