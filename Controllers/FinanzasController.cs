using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Services;

namespace SIS_DIAF.Controllers
{
    [Authorize(Roles = "finanzas")]
    public class FinanzasController : Controller
    {
        private readonly RRegimen _regimen;
        private readonly ISesion _sesion;

        private int _idUsuario { set; get; } = 0;

        public FinanzasController(RRegimen r_regimen, ISesion s_sesion)
        {
            _regimen = r_regimen;
            _sesion = s_sesion;

            this._idUsuario = this._sesion.retornarIdentificadorUsuario();
        }

        public IActionResult Index()
        {
            

            return View();
        }
    }
}
