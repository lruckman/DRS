using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
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