using Microsoft.AspNetCore.Mvc;
using Web.Features.Documents;

namespace Web.Features.Home
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction(nameof(DocumentsController.Index), "Documents");
        }
    }
}