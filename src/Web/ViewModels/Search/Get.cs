using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Engine;
using Web.Engine.Services.Lucene;
using Web.Models;

namespace Web.ViewModels.Search
{
    public class Get
    {
        public class Query : IAsyncRequest<IEnumerable<Result>>
        {
            public string Q { get; set; }

            public int Page { get; set; }

            public string Sort { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, IEnumerable<Result>>
        {
            private readonly ApplicationDbContext _db;
            private readonly ISearcher _searcher;

            public QueryHandler(ApplicationDbContext db, ISearcher searcher)
            {
                _db = db;
                _searcher = searcher;
            }

            public async Task<IEnumerable<Result>> Handle(Query message)
            {
                var documentIds = _searcher
                    .Search(message.Q);

                return await _db.Documents
                    .Where(d => documentIds.Contains(d.Id))
                    .Skip(Constants.SearchResultsPageSize * message.Page)
                    .Take(Constants.SearchResultsPageSize)
                    .ProjectTo<Result>()
                    .ToArrayAsync();
            }
        }

        public class Result
        {
            public int DocumentId { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }
            public long FileSize { get; set; }
            public int PageCount { get; set; }
        }
    }
}