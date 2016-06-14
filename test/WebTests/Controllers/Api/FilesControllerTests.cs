using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers.Api;
using Web.Engine.Helpers;
using Web.Models;
using Web.ViewModels.Api.Files;
using Xunit;

namespace WebTests.Controllers.Api
{
    public class FilesControllerTests : BaseTest
    {
        [Fact]
        public void ControllerThrowsExceptionWhenArgumentsAreNull()
        {
            Assert.Throws<ArgumentNullException>(() => new FilesController(null));
        }

        [Fact]
        public void ViewModelFailsValidation()
        {
            var model = new View.Query {Id = null};
            var documentSecurity = new Mock<IDocumentSecurity>();

            documentSecurity
                .Setup(x => x.HasFilePermissionAsync(It.IsAny<int>(), It.IsAny<PermissionTypes>()))
                .Returns(Task.FromResult(It.IsAny<bool>()));

            var validationResults = new View.QueryValidator(documentSecurity.Object)
                .Validate(model);

            Assert.False(validationResults.IsValid);
        }

        [Fact]
        public async Task ViewReturnsNotFoundWhenModelIsNull()
        {
            var model = new View.Query {Id = 1};
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(model))
                .Returns(Task.FromResult((View.Result) null));

            var controller = new FilesController(mediator.Object);
            var result = await controller.View(It.IsAny<View.Query>());

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ViewReturnsCorrectModelType()
        {
            var mediator = new Mock<IMediator>();

            mediator
                .Setup(x => x.SendAsync(It.IsAny<View.Query>()))
                .Returns(Task.FromResult(new View.Result { ContentType = TestFile1ContentType, FileContents = TestFile1Bytes}));

            var controller = new FilesController(mediator.Object);
            var result = await controller.View(It.IsAny<View.Query>());

            Assert.IsType<FileContentResult>(result);
        }
    }
}