using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels.Api.Search;

namespace Web.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class SearchController : BaseController
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        public async Task<IActionResult> Get(Get.Query query)
        {
            var results = await _mediator.SendAsync(query);

            return Ok(results);
        }
    }
}