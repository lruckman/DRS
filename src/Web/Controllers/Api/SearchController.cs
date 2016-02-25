using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNet.Mvc;
using Web.ViewModels.Api.Search;

namespace Web.Controllers.Api
{
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
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var results = await _mediator.SendAsync(query);

            return Ok(results);
        }
    }
}