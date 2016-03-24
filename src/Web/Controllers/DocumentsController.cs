using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;

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