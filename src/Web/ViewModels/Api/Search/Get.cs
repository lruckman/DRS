using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Engine;
using Web.Models;

namespace Web.ViewModels.Api.Search
{
    public class Get
    {
        public class Query : IAsyncRequest<Result>
        {
            public int?[] LibraryIds { get; set; }

            public string Q { get; set; }

            public int PageIndex { get; set; }

            public string OrderBy { get; set; }

            public int MaxResults { get; set; } = Constants.SearchResultsPageSize;
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.MaxResults)
                    .InclusiveBetween(0, 100);
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _configurationProvider;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider configurationProvider)
            {
                _db = db;
                _configurationProvider = configurationProvider;
            }

            public async Task<Result> Handle(Query message)
            {
                var documentQuery = _db.Documents
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(message.Q))
                {
                    documentQuery = documentQuery
                        .FromSql(
                            $"SELECT d.* FROM [dbo].[{nameof(Document)}] AS d JOIN FREETEXTTABLE([dbo].[vDocumentSearch], *, @p0) AS s ON d.Id = s.[Key]",
                            message.Q);
                }

                var result = new Result
                {
                    TotalCount = await documentQuery
                        .CountAsync()
                };

                if (result.TotalCount > message.MaxResults * (message.PageIndex + 1))
                {
                    result.NextLink =
                        $"/api/search/?{nameof(message.Q)}={message.Q}{string.Join($"&{nameof(message.LibraryIds)}=", message.LibraryIds)}&{nameof(message.OrderBy)}={message.OrderBy}&{nameof(message.PageIndex)}={message.PageIndex + 1}";
                }

                result.Documents = await documentQuery
                    .OrderBy(d => d.Id)
                    .Skip(message.MaxResults * message.PageIndex)
                    .Take(message.MaxResults)
                    .ProjectTo<Result.Document>(_configurationProvider)
                    .ToArrayAsync();

                return result;
            }

            public class MappingProfile : Profile
            {
                protected override void Configure()
                {
                    CreateMap<Document, Result.Document>()
                        .ForMember(d => d.SelfLink, o => o.MapFrom(s => $"/api/documents/{s.Id}"))
                        .ForMember(d => d.ThumbnailLink, o => o.MapFrom(s => $"/api/documents/{s.Id}/thumbnail"))
                        .ForMember(d => d.ViewLink, o => o.MapFrom(s => $"/api/documents/{s.Id}/view")); ;
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
                public string Title { get; set; }
                public string SelfLink { get; set; }
                public string ViewLink { get; set; }
                public string ThumbnailLink { get; set; }
                public string Abstract { get; set; }
            }
        }
    }
}