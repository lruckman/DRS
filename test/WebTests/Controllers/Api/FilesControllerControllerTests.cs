using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Features.Api.Files;
using Xunit;

namespace WebTests.Controllers.Api
{
    public class FilesControllerControllerTests : BaseControllerTest<FilesController>
    {
        [Fact]
        public void Controller_MissingArgument_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => new FilesController(null));
        }

        [Fact]
        public async Task View_NullModel_ReturnsNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var controller = CreateController(mediator.Object);
            var result = await controller.View(It.IsAny<View.Query>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task View_ReturnsCorrectModelType()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<View.Query>()))
                .Returns(
                    Task.FromResult(new View.Result {ContentType = TestFile1ContentType, FileContents = TestFile1Bytes}));

            var controller = new FilesController(mediator.Object);
            var result = await controller.View(It.IsAny<View.Query>());

            Assert.IsType<FileContentResult>(result);
        }

        [Fact]
        public async Task Thumbnail_NullModel_ReturnsNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var controller = CreateController(mediator.Object);
            var result = await controller.Thumbnail(It.IsAny<Thumbnail.Query>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Thumbnail_ReturnsCorrectModelType()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<Thumbnail.Query>()))
                .Returns(Task.FromResult(new Thumbnail.Result {FileContents = TestFile1Bytes}));

            var controller = CreateController(mediator.Object);
            var result = await controller.Thumbnail(It.IsAny<Thumbnail.Query>());

            Assert.IsType<FileContentResult>(result);
        }
    }
}