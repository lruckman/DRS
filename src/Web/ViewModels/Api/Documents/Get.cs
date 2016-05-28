using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Web.Engine.Helpers;
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
            private readonly IDocumentSecurity _documentSecurity;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider configurationProvider,
                IDocumentSecurity documentSecurity)
            {
                _db = db;
                _configurationProvider = configurationProvider;
                _documentSecurity = documentSecurity;
            }

            public async Task<Result> Handle(Query message)
            {
                if (!await _documentSecurity.HasDocumentPermissionAsync(message.Id.Value, PermissionTypes.Read))
                {
                    return new Result {Status = Result.StatusTypes.FailureUnauthorized};
                }

                var userLibraryIds = await _documentSecurity.GetUserLibraryIdsAsync(PermissionTypes.Read);

                return await _db.Documents
                    .Where(d => d.Id == message.Id && d.Libraries.Any(l => userLibraryIds.Contains(l.LibraryId)))
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
                                .Select(l => l.LibraryId.ToString())))
                        .ForMember(d => d.Status, o => o.MapFrom(s => Result.StatusTypes.Success));
                    CreateMap<File, Result.FileResult>();
                }
            }
        }

        public class Result
        {
            public enum StatusTypes
            {
                FailureUnauthorized,
                Success
            }

            public StatusTypes Status { get; set; }

            public int Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public string CreatedOn { get; set; }
            public string ModifiedOn { get; set; }
            public string[] LibraryIds { get; set; } = {};
            public FileResult File { get; set; }

            public class FileResult
            {
                public int Id { get; set; }
                public long Size { get; set; }
                public int PageCount { get; set; }
                public string ThumbnailLink => $"/api/files/{Id}/thumbnail";
                public string CreatedOn { get; set; }
                public string ModifiedOn { get; set; }
                public int VersionNum { get; set; }
            }
        }
    }
}