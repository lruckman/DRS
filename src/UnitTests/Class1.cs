using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.Mvc;
using Web.Controllers;
using Web.ViewModels.Documents;
using Xunit;

namespace UnitTests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Class1
    {
        [Fact]
        public async Task CreateDocument()
        {
            IMediator mediator = null;
            var controller = new DocumentsController(mediator);
            var command = new Create.Command();

            var actionResult = await controller.Create(command);

            Assert.IsType<IActionResult>(actionResult);
        }
    }
}