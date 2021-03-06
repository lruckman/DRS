﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Engine;
using Web.Engine.Helpers;
using Web.Engine.Services;
using Web.Engine.Validation.Custom;
using Web.Models;

namespace Web.Features.Api.Documents
{
    public class Index
    {
        public class Query : IRequest<Result>
        {
            public int[] LibraryIds { get; set; } = { };

            public string Keywords { get; set; }

            public int PageIndex { get; set; }

            public string OrderBy { get; set; }

            public int MaxResults { get; set; } = Constants.SearchResultsPageSize;
        }

        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator(IDocumentSecurity documentSecurity)
            {
                RuleFor(m => m.MaxResults)
                    .InclusiveBetween(Constants.SearchResultsPageSize, Constants.SearchResultsMaxPageSize);
                RuleFor(m => m.LibraryIds)
                    .HasLibraryPermission(documentSecurity, PermissionTypes.Read);
            }
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _config;
            private readonly IDocumentSecurity _documentSecurity;
            private readonly IFileSearcher _fileSearcher;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider config,
                IDocumentSecurity documentSecurity,
                IFileSearcher fileSearcher)
            {
                _db = db;
                _config = config;
                _documentSecurity = documentSecurity;
                _fileSearcher = fileSearcher;
            }

            public async Task<Result> Handle(Query message, CancellationToken cancellationToken)
            {
                var documentQuery = _db.PublishedRevisions
                    .Where(pr => pr.EndDate == null);

                if (!string.IsNullOrWhiteSpace(message.Keywords))
                {
                    var keywords = $"{message.Keywords.Trim('*')}*"; // cannot start search text witha '*', add a '*' to the end so we do a starts with

                    var ids = _fileSearcher.Search(keywords)
                        .ToArray();

                    documentQuery = documentQuery.Where(dq => ids.Contains(dq.DocumentId));
                }

                if (message.LibraryIds.Length == 0)
                {
                    // no libraries so default to all the user libraries

                    var userLibraryIds = await _documentSecurity
                        .GetUserDistributionGroupIdsAsync(PermissionTypes.Read)
                        .ConfigureAwait(false);
                    //todo: roll this into fileindexer
                    message.LibraryIds = userLibraryIds
                        .Select(i => i)
                        .ToArray();
                }

                // limit based on libraries the user can access

                documentQuery = documentQuery
                    .Where(dq => dq.Document.Distributions.Any(l => message.LibraryIds.Contains(l.DistributionGroupId)));

                var result = new Result
                {
                    TotalCount = await documentQuery
                        .CountAsync()
                        .ConfigureAwait(false)
                };

                if (result.TotalCount > message.MaxResults * (message.PageIndex + 1))
                {
                    result.NextLink =
                        $"/api/documents/?{nameof(message.Keywords)}={message.Keywords}{string.Join($"&{nameof(message.LibraryIds)}=", message.LibraryIds)}&{nameof(message.OrderBy)}={message.OrderBy}&{nameof(message.PageIndex)}={(message.PageIndex + 1).ToString()}";
                }

                result.Documents = await documentQuery
                    .Skip(message.MaxResults * message.PageIndex)
                    .Take(message.MaxResults)
                    .ProjectTo<Result.DocumentResult>(_config)
                    .ToArrayAsync()
                    .ConfigureAwait(false);

                return result;
            }

            public class MappingProfile : AutoMapper.Profile
            {
                public MappingProfile()
                {
                    CreateMap<Revision, Result.DocumentResult>()
                        .ForMember(d => d.Id, o => o.MapFrom(s => s.DocumentId))
                        .ForMember(d => d.Version, o => o.MapFrom(s => s.VersionNum))
                        .ForMember(d => d.CreatedOn, o => o.MapFrom(s => s.Document.CreatedOn))
                        .ForMember(d => d.ModifiedOn, o => o.MapFrom(s => s.CreatedOn))
                        .ForMember(d => d.PageCount, o => o.MapFrom(s => s.DataFile.PageCount))
                        .ForMember(d => d.Size, o => o.MapFrom(s => s.DataFile.Size));
                }
            }
        }

        public class Result
        {
            public string NextLink { get; set; }
            public int TotalCount { get; set; }

            public IEnumerable<DocumentResult> Documents { get; set; }

            public class DocumentResult
            {
                public int Id { get; set; }
                public long Size { get; set; }
                public int PageCount { get; set; }
                public int Version { get; set; }

                public string ThumbnailLink => $"/api/documents/{Id.ToString()}/thumbnail";
                public string ViewLink => $"/api/documents/{Id.ToString()}/view";

                public string Abstract { get; set; }
                public string Title { get; set; }

                public DateTimeOffset ModifiedOn { get; set; }
                public DateTimeOffset CreatedOn { get; set; }
            }
        }
    }
}