using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
            public int? LibraryId { get; set; }

            public string Search { get; set; }

            public int Page { get; set; }

            public string Sort { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.LibraryId).NotNull();
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
                if (string.IsNullOrWhiteSpace(message.Search))
                {
                    return new Result[] {};
                }

                var documentIds = _searcher
                    .Search(message.Search);

                //todo: work around for bug, use Count()>0 rather then Any
                // https://github.com/aspnet/EntityFramework/issues/3317

                //todo: re-add library filter once EF fixes

                return await _db.Documents
                    .Where(d => documentIds.Contains(d.Id) /*&&
                                d.Libraries.Count(l => l.LibraryId == message.LibraryId.Value) > 0*/)
                    .OrderBy(d => d.Id)
                    .Skip(Constants.SearchResultsPageSize * message.Page)
                    .Take(Constants.SearchResultsPageSize)
                    .ProjectTo<Result>()
                    .ToArrayAsync();
            }

            public class MappingProfile : Profile
            {
                protected override void Configure()
                {
                    Mapper.CreateMap<Document, Result>()
                        .ForMember(d => d.ThumbnailUrl, o => o.MapFrom(s => $"/documents/{s.Id}/thumbnail"));
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }
            public string ThumbnailUrl { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }
            public long FileSize { get; set; }
            public int PageCount { get; set; }
        }
    }
}