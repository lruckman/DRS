using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Engine;
using Web.Models;

namespace Web.Features.Api.Profile
{
    public static class DistributionGroups
    {
        public class Query : IRequest<Result[]>
        {
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }

        public class QueryHandler : IAsyncRequestHandler<Query, Result[]>
        {
            private readonly ApplicationDbContext _db;
            private readonly IConfigurationProvider _config;
            private readonly IUserContext _userContext;

            public QueryHandler(ApplicationDbContext db,
                IConfigurationProvider config,
                IUserContext userContext)
            {
                _db = db;
                _config = config;
                _userContext = userContext;

                var test = _db.NamedDistributions
                    .Where(nd => nd.ApplicationUserId == _userContext.UserId)
                    .Select(nd => nd.DistributionGroup)
                    .Where(dg => dg.Status == StatusTypes.Active);
            }

            public Task<Result[]> Handle(Query message) => _db.NamedDistributions
                    .Where(nd => nd.ApplicationUserId == _userContext.UserId)
                    .Select(nd => nd.DistributionGroup)
                    .Where(dg => dg.Status == StatusTypes.Active)
                    .ProjectTo<Result>(_config)
                .ToArrayAsync();

            public class MappingProfile : AutoMapper.Profile
            {
                public MappingProfile()
                {
                    CreateMap<DistributionGroup, Result>();
                }
            }
        }

        public class Result
        {
            public int Id { get; set; }

            public DateTimeOffset CreatedOn { get; set; }
            public DateTimeOffset ModifiedOn { get; set; }

            public string Name { get; set; }
        }
    }
}