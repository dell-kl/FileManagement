using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using SIS_DIAF.DTO;
using SIS_DIAF.Models;
using SIS_DIAF.Models.ModelViews;
using SIS_DIAF.Repositorios;

namespace SIS_DIAF.Controllers
{
    //con esta etiqueta controlamos nuestras vistas.
    [Authorize(Roles = "administrador")]
    public class AdministradorController : Controller
    {
        private readonly IRepositorio<Rol> __repository;
        private readonly IRepositorio<Usuario> __repositoryUsuario;
        private ICollection<Rol> listado = new List<Rol>();
        
        public AdministradorController(IRepositorio<Rol> repository, IRepositorio<Usuario> repositoryUsuario) { 
            this.__repository = repository;
            this.__repositoryUsuario = repositoryUsuario;
        }


        // GET: AdministradorController
        public ActionResult Index()
        {
            ViewData["LanzarLoading"] = "d-none";
            return View();
        }


    }
}
