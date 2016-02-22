using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Web.ViewModels.Api.Documents;

namespace Web.Controllers.Api
{
    [Route("api/[controller]")]
    public class DocumentsController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("{id:int?}")]
        public async Task<IActionResult> Get(Get.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return Ok(model);
        }

        [Route("{documentId:int?}/thumbnail")]
        public async Task<IActionResult> GetThumbnail(GetThumbnail.Query query)
        {
            var model = await _mediator.SendAsync(query);

            return File(model.Thumbnail, model.ContentType);
        }

        public async Task<IActionResult> Post(Post.Command command)
        {
            var documentId = await _mediator.SendAsync(command);

            if (documentId == null)
            {
                return new HttpStatusCodeResult((int) HttpStatusCode.InternalServerError);
            }

            return CreatedAtAction(nameof(Get), new RouteValueDictionary(new Get.Query {Id = documentId}), null);
        }
    }
}