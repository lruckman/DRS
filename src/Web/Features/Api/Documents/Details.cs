using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine.Helpers;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public static class Details
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

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _config;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider config)
            {
                _db = db;
                _config = config;
            }

            public Task<Result> Handle(Query request, CancellationToken cancellationToken) => _db.PublishedRevisions
                    .Where(pr => pr.DocumentId == request.Id
                            && pr.EndDate == null)
                    .ProjectTo<Result>(_config)
                    .SingleOrDefaultAsync();

            public class MappingProfile : AutoMapper.Profile
            {
                public MappingProfile()
                {
                    CreateMap<PublishedRevision, Result>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.DocumentId))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.Document.CreatedOn))
                        .ForMember(d => d.ModifiedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.PageCount, o => o.MapFrom(s => s.DataFile.PageCount))
                        .ForMember(d => d.Size, o => o.MapFrom(s => s.DataFile.Size))
                        .ForMember(d => d.LibraryIds, o => o.MapFrom(s =>
                            s.Document.Distributions.Select(d => d.DistributionGroupId).ToArray()))
                        .ForMember(d => d.Version, o => o.MapFrom(s => s.VersionNum));
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }

            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }

            public int[] LibraryIds { get; set; } = { }; // todo: rename
            public int PageCount { get; set; }
            public long Size { get; set; }
            public string ThumbnailLink => $"/api/documents/{Id}/thumbnail?v={Version}";
            public string ViewLink => $"/api/documents/{Id}/view";
            public int Version { get; set; }

            public string Abstract { get; set; }
            public string Title { get; set; }
        }
    }
}