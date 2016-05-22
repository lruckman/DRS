using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Engine;
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
            private readonly IUserAccessor _userAccessor;

            public CommandHandler(ApplicationDbContext db,
                IUserAccessor userAccessor)
            {
                _db = db;
                _userAccessor = userAccessor;
            }

            public async Task<int> Handle(Command message)
            {
                var document = await _db.Documents
                    .Include(d => d.Libraries)
                    .SingleAsync(d => d.Id == message.Id.Value);

                document.ModifiedOn = DateTimeOffset.Now;
                document.Title = message.Title;
                document.Abstract = message.Abstract;

                var userId = _userAccessor.UserId;

                // get all the user libraries

                var userLibraryIds = await _db.UserLibraries
                    .Where(ul => ul.ApplicationUserId == userId)
                    .Select(ul => ul.LibraryId)
                    .ToArrayAsync();

                // remove deleted libraries

                var deletedLibraryIds = document.Libraries
                    .Select(l => l.LibraryId)
                    .Except(message.LibraryIds)
                    .ToArray();

                document.Libraries.RemoveAll(ld => deletedLibraryIds.Contains(ld.LibraryId));

                // add new libraries

                var newLibraryIds = message.LibraryIds
                    .Except(document.Libraries.Select(l => l.LibraryId))
                    .Intersect(userLibraryIds)
                    .ToArray();

                document.Libraries.AddRange(newLibraryIds.Select(id => new LibraryDocument {LibraryId = id}));

                await _db.SaveChangesAsync();

                return document.Id;
            }
        }
    }
}