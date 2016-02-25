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

namespace Web.ViewModels.Api.Search
{
    public class Get
    {
        public class Query : IAsyncRequest<Result>
        {
            public int? LibraryId { get; set; }

            public string Q { get; set; }

            public int PageIndex { get; set; }

            public string OrderBy { get; set; }

            public int MaxResults { get; set; } = Constants.SearchResultsPageSize;
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.LibraryId)
                    .NotNull();
                RuleFor(m => m.MaxResults)
                    .InclusiveBetween(0, 100);
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly ISearcher _searcher;

            public QueryHandler(ApplicationDbContext db, ISearcher searcher)
            {
                _db = db;
                _searcher = searcher;
            }

            public async Task<Result> Handle(Query message)
            {
                if (string.IsNullOrWhiteSpace(message.Q))
                {
                    return new Result();
                }

                var documentIds = _searcher
                    .Search(message.Q);

                //todo: work around for bug, use Count()>0 rather then Any
                // https://github.com/aspnet/EntityFramework/issues/3317

                //todo: re-add library filter once EF fixes

                var query = _db.Documents
                    .Where(d => documentIds.Contains(d.Id) /*&&
                                d.Libraries.Count(l => l.LibraryId == message.LibraryId.Value) > 0*/);

                var result = new Result
                {
                    TotalCount = await query.CountAsync()
                };

                if (result.TotalCount > message.MaxResults * (message.PageIndex + 1))
                {
                    result.NextLink =
                        $"/api/search/?q={message.Q}&libraryid={message.LibraryId}&orderby={message.OrderBy}&pageindex={message.PageIndex + 1}";
                }

                result.Documents = await query
                    .OrderBy(d => d.Id)
                    .Skip(message.MaxResults * message.PageIndex)
                    .Take(message.MaxResults)
                    .ProjectTo<Result.Document>()
                    .ToArrayAsync();

                return result;
            }

            public class MappingProfile : Profile
            {
                protected override void Configure()
                {
                    Mapper.CreateMap<Document, Result.Document>()
                        .ForMember(d => d.ThumbnailLink, o => o.MapFrom(s => $"/api/documents/{s.Id}/thumbnail"))
                        .ForMember(d => d.ViewLink, o => o.MapFrom(s => $"/api/documents/{s.Id}/view"));
                }
            }
        }

        public class Result
        {
            public string NextLink { get; set; }
            public int TotalCount { get; set; }

            public IEnumerable<Document> Documents { get; set; }

            public class Document
            {
                public int Id { get; set; }
                public string ViewLink { get; set; }
                public string ThumbnailLink { get; set; }
                public DateTimeOffset CreatedOn { get; set; }
                public DateTimeOffset ModifiedOn { get; set; }
                public long FileSize { get; set; }
                public int PageCount { get; set; }
            }
        }
    }
}