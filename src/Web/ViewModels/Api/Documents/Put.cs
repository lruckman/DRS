using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Models;

namespace Web.ViewModels.Api.Documents
{
    public class Put
    {
        public class Command : IAsyncRequest<int>
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

        public class CommandHandler : IAsyncRequestHandler<Command, int>
        {
            private readonly ApplicationDbContext _db;

            public CommandHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<int> Handle(Command message)
            {
                var document = await _db.Documents
                    .Include(d => d.Libraries)
                    .SingleAsync(d => d.Id == message.Id.Value);

                document.ModifiedOn = DateTimeOffset.Now;
                document.Title = message.Title;
                document.Abstract = message.Abstract;

                // remove deleted libraries

                document.Libraries.RemoveAll(ld => !message.LibraryIds.Contains(ld.LibraryId));

                // add new libraries

                var newLibraries = await _db.Libraries
                    .Where(l => message.LibraryIds.Contains(l.Id)
                                && !document.Libraries
                                    .Select(ld => ld.LibraryId)
                                    .Contains(l.Id))
                    .Select(l => new LibraryDocument
                    {
                        LibraryId = l.Id
                    })
                    .ToArrayAsync();

                document.Libraries.AddRange(newLibraries);

                await _db.SaveChangesAsync();

                return document.Id;
            }
        }
    }
}