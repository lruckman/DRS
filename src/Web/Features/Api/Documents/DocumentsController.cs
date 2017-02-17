using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Web.Controllers;

namespace Web.Features.Api.Documents
{
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentsController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Index(Index.Query query)
        {
            var result = await _mediator.Send(query).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create.Command command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);

            return CreatedAtAction(nameof(Documents.Index)
                , new RouteValueDictionary(new Index.Query {Id = result.DocumentId}), null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Edit(Edit.Command command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return await Index(new Index.Query {Id = result.DocumentId}).ConfigureAwait(false);
        }

        [HttpGet("{id:int}/thumbnail")]
        public async Task<IActionResult> Thumbnail(Thumbnail.Query query)
        {
            var model = await _mediator.Send(query).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }

            return File(model.FileContents, model.ContentType);
        }

        [HttpGet("{id:int}/view")]
        public async Task<IActionResult> View(View.Query query)
        {
            var model = await _mediator.Send(query).ConfigureAwait(false);

            if (model == null)
            {
                return NotFound();
            }

            return File(model.FileContents, model.ContentType);
        }
    }
}