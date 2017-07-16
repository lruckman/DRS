using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    /// <summary>
    /// TODO: investigate why this doesn't work in the features folder
    /// </summary>
    public class HomeController : BaseController
    {
        public IActionResult Index() => Redirect("~/documents");
    }
}