using System.Threading.Tasks;
using AutoMapper;
using Web.ViewModels.Api.Documents;
using Xunit;

namespace WebTests.ViewModels.Api.Documents
{
    public class GetTests : BaseViewModelTest
    {
        [Fact]
        public async Task QueryHandler_ReturnsCorrectModelType()
        {
            var db = CreateDbContext();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<Get.QueryHandler.MappingProfile>());
            var queryHandler = new Get.QueryHandler(db, config);
            var query = new Get.Query {Id = 1};
            var result = await queryHandler.Handle(query);

            Assert.IsType<Get.Result>(result);
        }
    }
}