using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.Data.Entity;
using Web.Models;

namespace Web.ViewModels.Documents
{
    public class Index
    {
        public class Query : IAsyncRequest<IEnumerable<Result>>
        {
            public int LibraryId { get; set; }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, IEnumerable<Result>>
        {
            private readonly ApplicationDbContext _db;

            public QueryHandler(ApplicationDbContext db)
            {
                _db = db;
            }

            public async Task<IEnumerable<Result>> Handle(Query message)
            {
                var model = await _db.Documents
                    .Where(d => d.Libraries.Any(l => l.LibraryId == message.LibraryId))
                    .ProjectTo<Result>()
                    .ToArrayAsync();

                return model;
            }

            public class MappingProfile : Profile
            {
                protected override void Configure()
                {
                    Mapper.CreateMap<Document, Result>()
                        .ForMember(d => d.DocumentId, o => o.MapFrom(s => s.Id));
                }
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