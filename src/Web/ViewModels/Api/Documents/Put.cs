using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Engine.Helpers;
using Web.Models;

namespace Web.ViewModels.Api.Documents
{
    public class Put
    {
        public class Command : IAsyncRequest<Result>
        {
            public int? Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public int[] LibraryIds { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(ApplicationDbContext db)
            {
                RuleFor(m => m.Id)
                    .NotNull();
                RuleFor(m => m.Abstract)
                    .Length(0, 512);
                RuleFor(m => m.LibraryIds)
                    .NotNull()
                    .MustAsync((libraryIds, cancellationToken) =>
                        db.Libraries.AllAsync(l => libraryIds.Contains(l.Id)));
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
                if (!await _documentSecurity.HasDocumentPermissionAsync(message.Id.Value, PermissionTypes.Modify))
                {
                    return new Result {Status = Result.StatusTypes.FailureUnauthorized};
                }

                var document = await _db.Documents
                    .Include(d => d.Libraries)
                    .SingleAsync(d => d.Id == message.Id.Value);

                document.ModifiedOn = DateTimeOffset.Now;
                document.Title = message.Title;
                document.Abstract = message.Abstract;

                // remove deleted libraries

                var deletedLibraryIds = document.Libraries
                    .Select(l => l.LibraryId)
                    .Except(message.LibraryIds)
                    .ToArray();

                document.Libraries.RemoveAll(ld => deletedLibraryIds.Contains(ld.LibraryId));

                // add new libraries

                var newLibraryIds = message.LibraryIds
                    .Except(document.Libraries.Select(l => l.LibraryId))
                    .Intersect(await _documentSecurity.GetUserLibraryIdsAsync(PermissionTypes.Modify))
                    .ToArray();

                document.Libraries.AddRange(newLibraryIds.Select(id => new LibraryDocument {LibraryId = id}));

                await _db.SaveChangesAsync();

                return new Result { DocumentId = document.Id };
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
            public int DocumentId { get; set; }
        }
    }
}