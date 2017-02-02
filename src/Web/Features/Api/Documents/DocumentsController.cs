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
        public async Task<IActionResult> Get(Get.Query query)
        {
            var result = await _mediator.Send(query).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post.Command command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);

            return CreatedAtAction(nameof(Documents.Get)
                , new RouteValueDictionary(new Get.Query {Id = result.DocumentId}), null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(Put.Command command)
        {
            var result = await _mediator.Send(command).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return await Get(new Get.Query {Id = result.DocumentId}).ConfigureAwait(false);
        }
    }
}