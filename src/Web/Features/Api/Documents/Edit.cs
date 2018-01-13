using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine;
using Web.Engine.Helpers;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public static class Edit
    {
        public class Command : IRequest<Result>
        {
            public int? Id { get; set; }
            public string Title { get; set; }
            public string Abstract { get; set; }
            public int[] LibraryIds { get; set; } = { };
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Id)
                    .NotNull()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Modify);

                RuleFor(m => m.Abstract)
                    .Length(0, 512);

                //RuleFor(m => m.LibraryIds)
                //    .NotEmpty().WithName("Libraries")
                //    .HasLibraryPermission(documentSecurity, PermissionTypes.Modify);

                RuleFor(m => m.Title)
                    .NotNull()
                    .Length(1, 60);
            }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IDocumentSecurity _documentSecurity;
            private readonly IUserContext _userContext;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext db, IDocumentSecurity documentSecurity, IUserContext userContext,
                IMapper mapper)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _documentSecurity = documentSecurity ?? throw new ArgumentNullException(nameof(documentSecurity));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            }

            public async Task<Result> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentVersion = await _db.PublishedRevisions
                    .Include(f => f.Document)
                    .Include(f => f.Document.Distributions)
                    .SingleOrDefaultAsync(f => f.DocumentId == request.Id.Value && f.EndDate == null)
                    .ConfigureAwait(false);

                if (currentVersion == null)
                {
                    return null;
                }

                if (currentVersion.Title != request.Title
                    || currentVersion.Abstract != request.Abstract)
                {
                    currentVersion.EndDate = DateTimeOffset.Now;

                    var newVersion = _mapper.Map<PublishedRevision>(currentVersion);

                    newVersion.VersionNum = await _db.Revisions
                        .Where(r => r.DocumentId == request.Id.Value)
                        .MaxAsync(d => d.VersionNum)
                        .ConfigureAwait(false);

                    newVersion.VersionNum++;

                    newVersion.Abstract = request.Abstract;
                    newVersion.Title = request.Title;
                    newVersion.CreatedBy = _userContext.UserId;
                    newVersion.CreatedOn = DateTimeOffset.Now;

                    _db.PublishedRevisions.Add(newVersion);
                }

                //// remove deleted libraries

                //var deletedLibraryIds = currentVersion.Document.Distributions
                //    .Select(l => l.DistributionGroupId)
                //    .Except(message.LibraryIds)
                //    .ToArray();

                //currentVersion.Document.Distributions.RemoveAll(ld => deletedLibraryIds.Contains(ld.DistributionGroupId));

                //// add new libraries

                //var second = await _documentSecurity.GetUserDistributionGroupIdsAsync(PermissionTypes.Modify)
                //    .ConfigureAwait(false);

                //var newLibraryIds = message.LibraryIds
                //    .Except(currentVersion.Document.Distributions.Select(l => l.DistributionGroupId))
                //    .Intersect(second)
                //    .ToArray();

                //currentVersion.Document.Distributions.AddRange(newLibraryIds.Select(id => new Distribution { DistributionGroupId = id }));

                await _db.SaveChangesAsync().ConfigureAwait(false);

                return new Result { DocumentId = currentVersion.DocumentId };
            }

            public class MappingProfile : AutoMapper.Profile
            {
                public MappingProfile()
                {
                    CreateMap<PublishedRevision, PublishedRevision>()
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => (DateTimeOffset?)null))
                        .ForMember(d => d.IndexedOn, o => o.MapFrom(s => (DateTimeOffset?)null));
                }
            }
        }

        public class Result
        {
            public int DocumentId { get; set; }
        }
    }
}