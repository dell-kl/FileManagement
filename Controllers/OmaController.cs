using Microsoft.AspNetCore.Mvc;

namespace SIS_DIAF.Controllers
{
    public class OmaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
