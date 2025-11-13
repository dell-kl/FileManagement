using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.Models;
using SIS_DIAF.Models.ModelViews;
using SIS_DIAF.Repositorios;

namespace SIS_DIAF.Controllers
{
    public class HistoricoController : Controller
    {
        private readonly RHistorico _r_historico;
        private readonly RTipoArchivo _r_tipoArchivo;
        private readonly RRegimen _r_regimen;

        private ICollection<Historico> listado = new List<Historico>();

        public HistoricoController(RHistorico _rh, RTipoArchivo _rTA, RRegimen regimen) { 
            this._r_historico = _rh;
            this._r_tipoArchivo = _rTA;
            this._r_regimen = regimen;
        }

        [HttpGet]
        public IActionResult Index(string guid, string filtro, string nreg)
        {
            if (string.IsNullOrEmpty(guid)) RedirectToAction("Index","Regimens");

            var historico = (List<Historico>) _r_historico.filtro(guid).Result;
            
            var tipos = _r_tipoArchivo.Listar().Result;

            HistoricoViews hv = new HistoricoViews()
            {
                historico = historico,
                tipoArchivo = tipos,
                Guid = guid,
                Regimen = nreg
            };

            if (string.IsNullOrEmpty(filtro))
            {
                return View(hv);
            }
            else
            {
                hv.historico = historico.Where<Historico>(h => h.historico_tipoArchivo!.Equals(filtro)).ToList();
                return View(hv);
            }
        }
    }

}

