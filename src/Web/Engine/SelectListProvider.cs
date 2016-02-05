using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using Web.Models;

namespace Web.Engine
{
    public interface ISelectListProvider
    {
        Task<IEnumerable<SelectListItem>> GetLibraries(int userId = 0);
    }

    public class SelectListProvider : ISelectListProvider
    {
        private readonly ApplicationDbContext _db;

        public SelectListProvider(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<SelectListItem>> GetLibraries(int userId = 0)
        {
            return await _db.Libraries
                .OrderBy(l => l.Name)
                .ProjectTo<SelectListItem>()
                .ToArrayAsync();
        }

        public class MappingProfile : Profile
        {
            protected override void Configure()
            {
                Mapper.CreateMap<Library, SelectListItem>()
                    .ForMember(d => d.Text, o => o.MapFrom(s => s.Name))
                    .ForMember(d => d.Value, o => o.MapFrom(s => s.Id))
                    .ForMember(d => d.Disabled, o => o.Ignore())
                    .ForMember(d => d.Group, o => o.Ignore())
                    .ForMember(d => d.Selected, o => o.Ignore());
            }
        }
    }
}