using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using Web.Controllers;

namespace Web.Features.Api.Profile
{
    [Authorize]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("distributiongroups")]
        public async Task<IActionResult> DistributionGroups(DistributionGroups.Query query)
        {
            var result = await _mediator.Send(query).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}