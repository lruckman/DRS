using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Api;
using Web.ViewModels.Api.Search;
using Xunit;

namespace WebTests.Controllers.Api
{
    public class SearchControllerControllerTests : BaseControllerTest<SearchController>
    {
        [Fact]
        public void Controller_MissingArgument_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchController(null));
        }

        [Fact]
        public async Task Get_ReturnsCorrectModelType()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<Get.Query>()))
                .Returns(Task.FromResult(new Get.Result()));

            var controller = CreateController(mediator.Object);
            var result = await controller.Get(It.IsAny<Get.Query>());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Get_NullModel_ReturnsNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var controller = CreateController(mediator.Object);
            var result = await controller.Get(It.IsAny<Get.Query>());

            Assert.IsType<NotFoundResult>(result);
        }
    }
}