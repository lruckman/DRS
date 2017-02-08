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
            public int? V { get; set; }
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
                return await _db.Files
                    .Where(d => d.DocumentId == message.Id
                            && ((message.V == null && d.EndDate == null) || d.VersionNum == message.V.Value)
                            && d.Status != StatusTypes.Deleted)
                    .ProjectTo<Result>(_config)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);
            }

            public class MappingProfile : Profile
            {
                public MappingProfile()
                {
                    CreateMap<File, Result>()
                        .ForMember(d => d.Abstract, o => o.MapFrom(s =>
                            s.Metadata.Single(m => m.EndDate == null).Abstract))
                        .ForMember(d => d.Title, o => o.MapFrom(s =>
                            s.Metadata.Single(m => m.EndDate == null).Title))
                        .ForMember(d => d.ModifiedOn, o => o.MapFrom(s =>
                            s.Metadata.Single(m => m.EndDate == null).CreatedOn))
                        .ForMember(d => d.LibraryIds, o => o.MapFrom(s =>
                            s.Document.Distributions.Select(d => d.DistributionGroupId.ToString())));
                }
            }
        }

        public class Result
        {
            public int DocumentId { get; set; }

            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }

            public string[] LibraryIds { get; set; } = {}; // todo: rename
            public int PageCount { get; set; }
            public long Size { get; set; }
            public string ThumbnailLink => $"/api/documents/{DocumentId}/thumbnail?v={VersionNum}"; // todo: array?
            public int VersionNum { get; set; }

            public string Abstract { get; set; }
            public string Title { get; set; }
        }
    }
}