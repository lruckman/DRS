using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Engine.Services.Lucene;
using Web.Engine.Services.Lucene.Models;
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
            public int? LibraryIds { get; set; }
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
                    .MustAsync((libraryId, cancellationToken) =>
                        db.Libraries.AnyAsync(l => l.Id == libraryId.Value));
                RuleFor(m => m.Title)
                    .NotNull()
                    .Length(1, 60);
            }
        }

        public class CommandHandler : IAsyncRequestHandler<Command, int>
        {
            private readonly ApplicationDbContext _db;
            private readonly IIndexer _indexer;

            public CommandHandler(ApplicationDbContext db, IIndexer indexer)
            {
                _db = db;
                _indexer = indexer;
            }

            public async Task<int> Handle(Command message)
            {
                var libraries = await _db.Libraries
                    .Where(l => l.Id == message.LibraryIds.Value)
                    .ToArrayAsync();

                var document = await _db.Documents
                    .SingleAsync(d => d.Id == message.Id.Value);

                document.ModifiedOn = DateTimeOffset.Now;
                document.Title = message.Title;
                document.Abstract = message.Abstract;

                // add the document to the selected libraries

                document.Libraries.Clear();

                document.Libraries.AddRange(libraries.Select(l => new LibraryDocument
                {
                    Library = l
                }));

                var indexSuccessful = _indexer.Index(new Index.Command
                {
                    Id = document.Id,
                    Abstract = document.Abstract,
                    Contents = null, //todo: refetch document contents?
                    Title = document.Title
                });

                if (!indexSuccessful)
                {
                    //todo: something else
                    throw new Exception("Unable to update document index.");
                }

                await _db.SaveChangesAsync();

                return document.Id;
            }
        }
    }
}