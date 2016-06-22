using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Web.Models;
using Web.ViewModels.Api.Documents;
using Xunit;

namespace WebTests.ViewModels.Api.Documents
{
    public class GetTests
    {
        [Fact]
        public async Task QueryHandler_ReturnsCorrectModelType()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseInMemoryDatabase();
            optionsBuilder.EnableSensitiveDataLogging();

            var db = new ApplicationDbContext(optionsBuilder.Options);

            var config = new MapperConfiguration(cfg => cfg.AddProfile<Get.QueryHandler.MappingProfile>());
            var queryHandler = new Get.QueryHandler(db, config);
            var query = new Get.Query {Id = 1};
            var result = await queryHandler.Handle(query);

            Assert.IsType<Get.Result>(result);
        }
    }
}