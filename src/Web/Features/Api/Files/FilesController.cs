using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers;

namespace Web.Features.Api.Files
{
    [Authorize]
    [Route("api/[controller]")]
    public class FilesController : BaseController
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{id:int}/view")]
        public async Task<IActionResult> View(View.Query query)
        {
            var model = await _mediator.SendAsync(query);

            if (model == null)
            {
                return NotFound();
            }

            return File(model.FileContents, model.ContentType);
        }

        //[HttpGet("{id:int}")]
        //public async Task<IActionResult> Get(Get.Query query)
        //{
        //    var model = await _mediator.SendAsync(query);

        //    return Ok(model);
        //}

        [HttpGet("{id:int}/thumbnail")]
        public async Task<IActionResult> Thumbnail(Thumbnail.Query query)
        {
            var model = await _mediator.SendAsync(query);

            if (model == null)
            {
                return NotFound();
            }

            return File(model.FileContents, model.ContentType);
        }

        //[HttpPost]
        //public async Task<IActionResult> Post(Post.Command command)
        //{
        //    var documentId = await _mediator.SendAsync(command);

        //    if (documentId == null)
        //    {
        //        return new HttpStatusCodeResult((int) HttpStatusCode.InternalServerError);
        //    }

        //    return CreatedAtAction(nameof(ViewModels.Api.Documents.Get)
        //        , new RouteValueDictionary(new Get.Query {Id = documentId}), null);
        //}
    }
}