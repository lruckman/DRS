using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
            var model = await _mediator.SendAsync(query);

            if (model.Status == ViewModels.Api.Documents.Get.Result.StatusTypes.FailureUnauthorized)
            {
                return Unauthorized();
            }

            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Post.Command command)
        {
            var result = await _mediator.SendAsync(command);

            return CreatedAtAction(nameof(ViewModels.Api.Documents.Get)
                , new RouteValueDictionary(new Get.Query {Id = result.DocumentId}), null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(Put.Command command)
        {
            var result = await _mediator.SendAsync(command);

            if (result.Status == ViewModels.Api.Documents.Put.Result.StatusTypes.FailureUnauthorized)
            {
                return Unauthorized();
            }

            return await Get(new Get.Query {Id = result.DocumentId});
        }
    }
}