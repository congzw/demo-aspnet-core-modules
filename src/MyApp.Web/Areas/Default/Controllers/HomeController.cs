using Microsoft.AspNetCore.Mvc;

namespace MyApp.Web.Areas.Default.Controllers
{
    [Area("Default")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
