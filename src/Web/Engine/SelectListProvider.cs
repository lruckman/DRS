using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Engine
{
    public interface ISelectListProvider
    {
        Task<IEnumerable<SelectListItem>> GetLibraries(string userId);
    }

    public class SelectListProvider : ISelectListProvider
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfigurationProvider _configurationProvider;

        public SelectListProvider(ApplicationDbContext db, IConfigurationProvider configurationProvider)
        {
            _db = db;
            _configurationProvider = configurationProvider;
        }

        public async Task<IEnumerable<SelectListItem>> GetLibraries(string userId)
        {
            return await _db.UserLibraries
                .Where(ul => ul.ApplicationUserId == userId)
                .Select(ul => ul.Library)
                .OrderBy(l => l.Name)
                .ProjectTo<SelectListItem>(_configurationProvider)
                .ToArrayAsync();
        }

        public class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Library, SelectListItem>()
                    .ForMember(d => d.Text, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.Value, o => o.MapFrom(s => s.Id))
                    .ForMember(d => d.Disabled, o => o.Ignore())
                    .ForMember(d => d.Group, o => o.Ignore())
                    .ForMember(d => d.Selected, o => o.Ignore());
            }
        }
    }
}