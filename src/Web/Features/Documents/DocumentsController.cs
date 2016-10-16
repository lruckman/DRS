using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Controllers;

namespace Web.Features.Documents
{
    [Authorize]
    [Route("[controller]")]
    public class DocumentsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}