using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Engine.Helpers;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public class Get
    {
        public class Query : IRequest<Result>
        {
            public int? Id { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            private readonly IDocumentSecurity _documentSecurity;

            public QueryValidator(IDocumentSecurity documentSecurity)
            {
                _documentSecurity = documentSecurity;
            }

            public QueryValidator()
            {
                RuleFor(m => m.Id)
                    .NotNull()
                    .HasDocumentPermission(_documentSecurity, PermissionTypes.Read);
            }
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _config;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider config)
            {
                _db = db;
                _config = config;
            }

            public async Task<Result> Handle(Query message)
            {
                return await _db.Documents
                    .Where(d => d.Id == message.Id)
                    .ProjectTo<Result>(_config)
                    .SingleOrDefaultAsync();
            }

            public class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<Document, Result>()
                        .ForMember(d => d.File, o => o.MapFrom(s =>
                            s.Files
                                .Where(f => f.Status == StatusTypes.Active)
                                .OrderByDescending(f => f.VersionNum)
                                .Single()))
                        .ForMember(d => d.LibraryIds, o => o.MapFrom(s =>
                            s.Distributions
                                .Select(l => l.DistributionGroupId.ToString())));
                    CreateMap<File, Result.FileResult>();
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }
            public string[] LibraryIds { get; set; } = {};
            public FileResult File { get; set; }

            public class FileResult
            {
                public int Id { get; set; }
                public long Size { get; set; }
                public int PageCount { get; set; }
                public string ThumbnailLink => $"/api/files/{Id}/thumbnail";
                public DateTimeOffset CreatedOn { get; set; }
                public DateTimeOffset ModifiedOn { get; set; }
                public int VersionNum { get; set; }
            }
        }
    }
}