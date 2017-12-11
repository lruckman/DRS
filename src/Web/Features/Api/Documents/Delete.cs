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
    public static class Delete
    {
        public class Command : IRequest<Result>
        {
            public int Id { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.Id)
                    .NotNull()
                    .HasDocumentPermission(documentSecurity, PermissionTypes.Delete);
            }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IUserContext _userContext;
            private readonly IMapper _mapper;

            public CommandHandler(ApplicationDbContext db, IUserContext userContext,
                IMapper mapper)
            {
                _db = db ?? throw new ArgumentNullException(nameof(db));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            }

            public async Task<Result> Handle(Command message, CancellationToken cancellationToken)
            {
                var currentVersion = await _db.PublishedRevisions
                    .SingleOrDefaultAsync(r => r.DocumentId == message.Id && r.EndDate == null)
                    .ConfigureAwait(false);

                currentVersion.EndDate = DateTimeOffset.Now;

                var newVersion = _mapper.Map<DeletedRevision>(currentVersion);

                newVersion.VersionNum = await _db.Revisions
                    .Where(r => r.DocumentId == currentVersion.DocumentId)
                    .MaxAsync(d => d.VersionNum)
                    .ConfigureAwait(false);

                newVersion.VersionNum++;
                newVersion.CreatedBy = _userContext.UserId;
                newVersion.CreatedOn = DateTimeOffset.Now;

                _db.DeletedRevisions.Add(newVersion);

                await _db.SaveChangesAsync()
                    .ConfigureAwait(false);

                return new Result { Id = message.Id };
            }

            public class MappingProfile : AutoMapper.Profile
            {
                public MappingProfile()
                {
                    CreateMap<PublishedRevision, DeletedRevision>()
                        .ForMember(d => d.Status, o => o.MapFrom(s => (int)StatusTypes.Deleted))
                        .ForMember(d => d.EndDate, o => o.MapFrom(s => (DateTimeOffset?)null));
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }
        }
    }
}