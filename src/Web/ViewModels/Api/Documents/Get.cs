using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.Data.Entity;
using Web.Models;

namespace Web.ViewModels.Api.Documents
{
    public class Get
    {
        public class Query : IAsyncRequest<Result>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(m => m.Id)
                    .NotNull();
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _configurationProvider;

            public QueryHandler(ApplicationDbContext db, IConfigurationProvider configurationProvider)
            {
                _db = db;
                _configurationProvider = configurationProvider;
            }

            public async Task<Result> Handle(Query message)
            {
                return await _db.Documents
                    .Where(d => d.Id == message.Id)
                    .ProjectTo<Result>(_configurationProvider)
                    .SingleOrDefaultAsync();
            }

            public class MappingProfile : Profile
            {
                protected override void Configure()
                {
                    CreateMap<Document, Result>()
                        .ForMember(d => d.File, o => o.MapFrom(s =>
                            s.Files
                                .Where(f => f.Status == StatusTypes.Active)
                                .OrderByDescending(f => f.VersionNum)
                                .Single()))
                        .ForMember(d => d.LibraryIds, o => o.MapFrom(s =>
                            s.Libraries
                                .Select(l => l.LibraryId.ToString())));
                    CreateMap<File, Result.FileResult>();
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public string CreatedOn { get; set; }
            public string ModifiedOn { get; set; }
            public string[] LibraryIds { get; set; }
            public FileResult File { get; set; }

            public class FileResult
            {
                public int Id { get; set; }
                public long Size { get; set; }
                public int PageCount { get; set; }
                public string ThumbnailLink => $"/api/files/{Id}/thumbnail";
            }
        }
    }
}