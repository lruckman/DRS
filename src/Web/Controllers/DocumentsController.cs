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

        public async Task<IActionResult> Index(Index.Query query)
        {
            query.LibraryId = 2; //todo: remove hardcode
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        [Route("{documentId:int?}/[action]")]
        [ResponseCache(Duration = 3600)]
        public async Task<IActionResult> Thumbnail(Thumbnail.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return File(model.Thumbnail, model.ContentType);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Create(Create.Query query)
        {
            var model = await _mediator.SendAsync(query);
            return View(model);
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Create(Create.Command command)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Create));
            }

            var model = await _mediator.SendAsync(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Route("{documentId:int?}/[action]")]
        public async Task<IActionResult> View(View.Query query)
        {
            var model = await _mediator.SendAsync(query);
            return File(model.Document, model.ContentType);
        }
    }
}