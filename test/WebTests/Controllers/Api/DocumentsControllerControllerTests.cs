using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Api;
using Web.ViewModels.Api.Documents;
using Xunit;

namespace WebTests.Controllers.Api
{
    public class DocumentsControllerControllerTests : BaseControllerTest<DocumentsController>
    {
        [Fact]
        public void Controller_MissingArgument_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new DocumentsController(null));
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

        [Fact]
        public async Task Post_ReturnsCorrectModelType()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<Post.Command>()))
                .Returns(Task.FromResult(new Post.Result()));

            var controller = CreateController(mediator.Object);
            var result = await controller.Post(It.IsAny<Post.Command>());

            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task Post_NullModel_ReturnsException()
        {
            var mediator = new Mock<IMediator>();
            var controller = CreateController(mediator.Object);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await controller.Post(null));
        }
    }
}