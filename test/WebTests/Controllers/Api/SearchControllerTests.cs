using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Api;
using Web.Engine.Helpers;
using Web.Models;
using Web.ViewModels.Api.Search;
using Xunit;

namespace WebTests.Controllers.Api
{
    public class SearchControllerTests
    {
        [Fact]
        public void GetModelReturnsInvalid()
        {
            var model = new Get.Query {MaxResults = 200};
            var documentSecurity = new Mock<IDocumentSecurity>();

            documentSecurity
                .Setup(x => x.GetUserLibraryIdsAsync(It.IsAny<PermissionTypes>()))
                .Returns(Task.FromResult(Enumerable.Range(0, 0)));

            var validationResults = new Get.QueryValidator(documentSecurity.Object)
                .Validate(model);

            Assert.False(validationResults.IsValid);
        }

        [Fact]
        public void ControllerThrowsExceptionWhenArgumentsAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchController(null));
        }

        [Fact]
        public async Task GetReturnsOkObjectResult()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<Get.Query>()))
                .Returns(Task.FromResult(new Get.Result()));

            var controller = new SearchController(mediator.Object);

            var result = await controller.Get(It.IsAny<Get.Query>());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetReturnsNotFoundResult()
        {
            var mediator = new Mock<IMediator>();

            var controller = new SearchController(mediator.Object);

            var result = await controller.Get(It.IsAny<Get.Query>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetReturnsCorrectResponse()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<Get.Query>()))
                .Returns(Task.FromResult(new Get.Result()));

            var controller = new SearchController(mediator.Object);

            var result = await controller.Get(It.IsAny<Get.Query>()) as OkObjectResult;

            Assert.Equal((int) HttpStatusCode.OK, result?.StatusCode.Value);
            Assert.IsType<Get.Result>(result.Value);
        }
    }
}