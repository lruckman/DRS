using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.Mvc;
using Web.ViewModels.Documents;

namespace Web.Controllers
{
    [Route("[controller]")]
    public class DocumentsController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        [HttpGet("{documentId:int?}/[action]")]
        public async Task<IActionResult> View(View.Query query)
        {
            var model = await _mediator.SendAsync(query);
            return File(model.Document, model.ContentType);
        }
    }
}