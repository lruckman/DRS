using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers;

namespace Web.Features.Api.Search
{
    [Authorize]
    [Route("api/[controller]")]
    public class SearchController : BaseController
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> Get(Get.Query query)
        {
            var results = await _mediator.Send(query).ConfigureAwait(false);

            if (results == null)
            {
                return NotFound();
            }

            return Ok(results);
        }
    }
}