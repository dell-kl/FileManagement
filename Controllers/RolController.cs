    using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SIS_DIAF.DTO;
using SIS_DIAF.Models;
using SIS_DIAF.Models.ModelViews;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Utilities;

namespace SIS_DIAF.Controllers
{

    [Authorize(Roles = "administrador")]
    public class RolController : Controller
    {
        private readonly IRepositorio<Rol> _repository;
        private int _pag = 5;
        public RolController(IRepositorio<Rol> repositorio) { 
            _repository = repositorio;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
         
            var listado = _repository.Listar().Result.ToList().Take(_pag);
            TempData["pagina"] = 0;
            TempData["b-back"] = "true";
            TempData["b-s-back"] = "disabled";
            return View(new RolesModelViews() { Roles = listado});
        }

        [HttpGet]
        public ActionResult Siguiente(int pagina)
        {
            var lista = _repository.Listar().Result;
            int pag = _pag * pagina;
            int cantidad = _pag;

            if(lista.Count()<= pag)
            {
                cantidad = pag - lista.Count();
            }

            var resultados = lista
                    .Skip(pag) // Omitir los primeros 5
                    .Take(cantidad) // Tomar los siguientes 5
                    .ToList();

            if (resultados.Count == 0)
            {
                TempData["b-next"] = "true";
                TempData["b-s-next"] = "disabled";
            }

            TempData["pagina"] = pagina;

            return View("Create", new RolesModelViews() { Roles = resultados });
        }
        
        [HttpGet]
        public ActionResult Atras(int pagina)
        {
            if(pagina <= 0)
            {
                return RedirectToAction("Create");
            }

            var resultados = _repository.Listar().Result
                    .Skip(_pag*pagina) // Omitir los primeros 5
                    .Take(5) // Tomar los siguientes 5
                    .ToList();

            TempData["pagina"] = pagina;

            return View("Create", new RolesModelViews() { Roles = resultados });
        
        }



        [HttpPost]
        public ActionResult CreateRol(RegistroRolDto registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Rol r = new Rol()
                    {
                        rol_id = 0,
                        rol_descripcion = registro.descripcion,
                        rol_estado = registro.estado,
                        rol_fecha_creacion = DateTime.Now,
                    };

                    bool resultado = _repository.Create(r).Result;

                    if (!resultado)
                        this.TempData["status"] = "error registrar";
                    else
                        this.TempData["status"] = "completado registrar";
                }
                else
                {
                    this.TempData["status"] = "registrar";
                    this.TempData["registroRol"] = JsonConvert.SerializeObject(registro);
                    this.TempData["ErrorModelState"] = JsonConvert.SerializeObject(Serializar(ModelState));
                }

                return RedirectToAction("Create", "Rol");
            }
            catch
            {
                return RedirectToAction("Create", "Rol");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegistroRolDto registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Rol r = new Rol()
                    {
                        rol_id = registro.id,
                        rol_descripcion = registro.descripcion,
                        rol_estado = registro.estado,
                        rol_fecha_creacion = DateTime.Now,
                    };

                    bool resultado = _repository.Update(r).Result;

                    if (!resultado)
                        this.TempData["status"] = "error editar";
                    else
                        this.TempData["status"] = "completado editar";
                }
                else
                {
                    this.TempData["status"] = registro.id.ToString();
                    this.TempData["registroRol"] = JsonConvert.SerializeObject(registro);
                    this.TempData["ErrorModelState"] = JsonConvert.SerializeObject(ModelState);
                }

                return RedirectToAction("Create", "Administrador");
            }
            catch
            {
                return RedirectToAction("Create", "Administrador");
            }
        }
        

        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                bool resultado = _repository.Delete(id).Result;

                if (!resultado)
                {
                    ViewBag.ErrorMessage = "error";
                }

                return View();
            }
            catch
            {
                return View();
            }
        }

        /* Funcion que nos permite serealizar nuestro ModelState */
        public static Dictionary<string, string[]> Serializar(ModelStateDictionary modelState)
        {
            Dictionary<string, string[]> mensajesError = new Dictionary<string, string[]>();

            /* aqui vamos a recorrer al diccionar */
            foreach( var i in modelState )
            {
                mensajesError[i.Key] = i.Value.Errors.Select(e => e.ErrorMessage).ToArray();
            }

            return mensajesError;
        }

        public static void Deserializar(Dictionary<string, string[]> errores, ModelStateDictionary modelState)
        {
            foreach(var k in errores)
            {
                foreach(var error in k.Value)
                {
                    modelState.AddModelError(k.Key, error);
                }
            }
        }
    }
}
