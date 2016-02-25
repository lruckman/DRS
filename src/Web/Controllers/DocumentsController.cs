using Microsoft.AspNet.Mvc;

namespace Web.Controllers
{
    [Route("[controller]")]
    public class DocumentsController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}