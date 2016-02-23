using Microsoft.AspNet.Mvc;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(DocumentsController.Index), "Documents");
        }
    }
}