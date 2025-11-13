using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SIS_DIAF.Controllers
{
    public class AccesoUsuarioController : Controller
    {
        // GET: AccesoUsuarioController
        public ActionResult Denegado()
        {
            return View();
        }

    }
}
