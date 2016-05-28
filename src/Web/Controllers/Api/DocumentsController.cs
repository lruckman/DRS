using System;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Web.Engine.Exceptions;
using Web.ViewModels.Api.Documents;

namespace Web.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class DocumentsController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            _mediator = mediator;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(Get.Query query)
        {
            try
            {
                var model = await _mediator.SendAsync(query);

                return Ok(model);
            }
            catch (UnauthorizedException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post.Command command)
        {
            var documentId = await _mediator.SendAsync(command);

            if (documentId == null)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError);
            }

            return CreatedAtAction(nameof(ViewModels.Api.Documents.Get)
                , new RouteValueDictionary(new Get.Query {Id = documentId}), null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(Put.Command command)
        {
            try
            {
                var documentId = await _mediator.SendAsync(command);

                return await Get(new Get.Query {Id = documentId});
            }
            catch (UnauthorizedException)
            {
                return Unauthorized();
            }
        }
    }
}