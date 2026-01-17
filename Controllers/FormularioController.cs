using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace SIS_DIAF.Controllers
{
    public class FormularioController : Controller
    {
        private readonly ILogger<FormularioController> _logger;
        private readonly IRepositorio<Usuario> __repository;
        private readonly ILogin __login;
        private readonly ISesion __sesion;

        private Dictionary<String, String> data { set; get; } = new Dictionary<String, String>();

        public FormularioController(ILogger<FormularioController> logger, IRepositorio<Usuario> repository, ILogin login, ISesion sesion)
        {
            this._logger = logger;
            this.__repository = repository;
            this.__login = login;
            this.__sesion = sesion;
        }

        public IActionResult Login()
        {
            //obtener la parte de la direccion del servidor de manera dinamica.
            // string url = Request.Host.Value;

            /*
            Response.Headers.Add("Content-Security-Policy",
                "style-src 'self' https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css https://fonts.googleapis.com/css2?family=Roboto:ital%2bwght@0%2b100%3b0%2b300%3b0%2b400%3b0%2b500%3b0%2b700%3b0%2b900%3b1%2b100%3b1%2b300%3b1%2b400%3b1%2b500%3b1,700%3b1,900&display=swap");
            */
            
            var rol = this.__sesion.verificarAutenticacion();
            
            if (!string.IsNullOrEmpty(rol))
            {
                return MostrarPerfil(rol);
            }
            
            return View();

        }

        private ActionResult MostrarPerfil(string rol)
        {
            rol = rol.ToLower();

            if (rol.Equals("administrador"))
            {
                return RedirectToAction("Index", "Administrador");
            }
            else if (rol.Equals("supervisor"))
            {
                return RedirectToAction("Index", "Regimens");

            } else if (rol.Equals("responsable"))
            {
                return RedirectToAction("Index", "Regimens");

            } else if (rol.Equals("logistica administrador"))
            {
                return RedirectToAction("Index", "LogisticaAdmin");
            }
            else if ( rol.Equals("logística") )
            {
                return RedirectToAction("Index", "Logistica");
            }
            else if ( rol.Equals("usuario final") ) 
            {
                return RedirectToAction("Index", "Regimens");
            }
            else if ( rol.Equals("finanzas") )
            {
                return RedirectToAction("Index", "Regimens");
            }
            else if ( rol.Equals("jurídico") )
            {
                return RedirectToAction("Index", "Regimens");
            }
            else{

                return RedirectToAction("Denegado","AccesoUsuario");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto formulario)
        {
            try
            {
                if ( ModelState.IsValid )
                {
                    PerfilUsuarioDto registro = await this.__login.Filtrar(formulario);
                    
                    if ( registro.tipo.Equals("sesion_correcta") )
                    {
                        var Rol = registro.rol;

                        //definir nuestra parte de la autenticacion.
                        this.__sesion.autenticacion(registro, formulario);

                       return MostrarPerfil(Rol);
                    }
                    
                    //estos mensajes van a encontrarse dentro de nuestro inicio de sesion.
                    ViewBag.ErrorMessage = registro.mensaje;
                    ViewBag.ErrorTipo = registro.tipo;
                }

                return View();
            }
            catch(Exception)
            {
                return View();
            }
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Formulario");
        }
    }
}
