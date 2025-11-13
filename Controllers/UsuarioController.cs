using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.DTO;
using SIS_DIAF.Models.ModelViews;
using SIS_DIAF.Models;
using SIS_DIAF.Repositorios;
using Microsoft.AspNetCore.Authorization;
using SIS_DIAF.Utilities;
using NuGet.Protocol;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.Security;
using System.Text.RegularExpressions;

namespace SIS_DIAF.Controllers
{
    [Authorize(Roles = "administrador")]
    public class UsuarioController : Controller
    {
        private readonly IRepositorio<Usuario> __repositoryUsuario;
        private readonly RUsuario _r_usuario;
        private readonly IRepositorio<Rol> _rol_rep;
        private readonly RSucuarsales _rSucuarsales;
        private readonly EncriptacionPass _security;
        private readonly GestionarEncriptado _gestionarEncriptado;

        public UsuarioController(
            IRepositorio<Usuario> repositorio,
            IRepositorio<Rol> rol_rep,
            RSucuarsales rSucursales,
            RUsuario usuario,
            EncriptacionPass security,
            GestionarEncriptado gestionarEncriptado
            )
        {
            this.__repositoryUsuario = repositorio;
            this._gestionarEncriptado = gestionarEncriptado;
            _rol_rep = rol_rep;
            _rSucuarsales = rSucursales;
            _r_usuario = usuario;
            _security = security;
           
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> ListaUsuarios(string tipo, string pagina, string direccion, string entrada)
        {
            /*
             Los parametros -> tipo, entrada <-
             son usados dentro del formulario de busqueda por coincidencia, es el filtrador para buscar mas rapido.
             */
            Dictionary<string, string> datosEncripts = new Dictionary<string, string>();
            datosEncripts.Add("pagina", pagina);
            datosEncripts.Add("tipo", tipo);
            datosEncripts.Add("direccion", direccion);
            datosEncripts.Add("entrada", entrada);
            datosEncripts = _gestionarEncriptado.SeguridadEncriptado(datosEncripts);

            if (datosEncripts == null)
                return RedirectToAction("ListaUsuarios");

            TempData["tipo"] = datosEncripts["tipo"];
            TempData["entrada"] = datosEncripts["entrada"];
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];


            ICollection<Usuario> listaUsuariosGeneral = new List<Usuario>();
            Dictionary<string, object> datos = new Dictionary<string, object>();

            if (int.Parse(TempData["tipo"]!.ToString()!) == 0 && TempData["entrada"]!.ToString()!.Equals("vacio"))
                listaUsuariosGeneral = await this.__repositoryUsuario.Listar();
            else
                listaUsuariosGeneral = await buscarXFiltro(int.Parse(TempData["tipo"]!.ToString()!), TempData["entrada"]!.ToString()!);

            if (direccion.Equals("siguiente") || direccion.Equals("inicio"))
                datos = Ordenar<Usuario>.Siguiente(int.Parse(pagina), listaUsuariosGeneral);
            else
                datos = Ordenar<Usuario>.Atras(int.Parse(pagina), listaUsuariosGeneral);


            ICollection<Usuario> listadoUsuariosRegistrado = new List<Usuario>(); 
            ICollection<Rol> listadoRoles = new List<Rol>();
            ICollection<Sucursal> listadoSucursal = new List<Sucursal>();

            listadoUsuariosRegistrado = (ICollection<Usuario>)datos["info"];
            listadoRoles = await _rol_rep.Listar();
            listadoSucursal = await _rSucuarsales.Listar();

            UsuariosModelViews usuariosModelViews = new UsuariosModelViews();
            usuariosModelViews.Usuarios =  listadoUsuariosRegistrado;
            usuariosModelViews.registroUsuarioDTO = new RegistroUsuarioDto();
            usuariosModelViews.listaRoles  = listadoRoles;
            usuariosModelViews.listaSucursales = listadoSucursal;

            TempData["b-back"] = datos["b-back"].ToString();
            TempData["b-s-back"] = datos["b-s-back"].ToString();

            TempData["b-next"] = datos["b-next"].ToString();
            TempData["b-s-next"] = datos["b-s-next"].ToString();
            
            TempData["info"] = datos["info"].ToString();
            TempData["pagina"] = _security.EncriptrarUrl(datos["pagina"].ToString()!);


            TempData["tipo"] = _security.EncriptrarUrl(TempData["tipo"]!.ToString()!);
            TempData["entrada"] = _security.EncriptrarUrl(TempData["entrada"]!.ToString()!);

            ViewData["LanzarLoading"] = "d-none";

            return View(usuariosModelViews);
        }


        [HttpPost]
        public ActionResult filtroUsuarios(string tipo, string entrada)
        {
            tipo = _security.EncriptrarUrl(tipo);
            entrada = _security.EncriptrarUrl(entrada);

            return RedirectToAction("ListaUsuarios", "Usuario", new { tipo = tipo, entrada = entrada });
        }


        //metodo que nos servira para poder filtrar de manera mucho mas rapido los datos.
        public async Task<ICollection<Usuario>> buscarXFiltro(int tipo, string entrada)
        {
            ICollection<Usuario> datosUsuario = new List<Usuario>();

            switch(tipo)
            {
                case 1:
                    datosUsuario.Add(await _r_usuario.GetxCedula(entrada));
                    break;

                case 2:
                    //vamos a tener que hacer un filtro por buscar todos por
                    //coincidencia de nombre o de letras...
                    datosUsuario = await _r_usuario.GetXCoincidencia(entrada);
                    break;
            }

            return datosUsuario;
        }


        [HttpPost]
        public async Task<ActionResult> Create(RegistroUsuarioDto registro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //verificar si ya existe un usuario con la cedula proporcionada. 
                    Usuario usuarioVerificarCedula = await _r_usuario.GetxCedula(registro.cedula);
                    bool resultado = false;

                    if (usuarioVerificarCedula == null)
                    {
                        //encriptar contrasena del nuevo usuario.
                        string claveEncriptada = registro.clave;
                        claveEncriptada = _security.EncriptarPassword(claveEncriptada);

                        Usuario u = new Usuario()
                        {
                            usuario_id = registro.id,
                            usuario_nombre = registro.nombre.ToUpper(),
                            usuario_cedula = registro.cedula,
                            usuario_email = registro.email,
                            usuario_guid = Guid.NewGuid(),
                            usuario_celular = registro.celular,
                            usuario_passwd = claveEncriptada,
                            usuario_estado = registro.estado,
                            RolId = registro.rol,
                            SucursalId = registro.sucursal,
                            usuario_nIntentos = 3

                        };

                        resultado = await __repositoryUsuario.Create(u);
                    }

                    if (!resultado)
                        this.TempData["status"] = "false";
                    else
                        this.TempData["status"] = "true";
                }
                return RedirectToAction("ListaUsuarios");
            }
            catch
            {
                this.TempData["status"] = "error registrar";
                return RedirectToAction("ListaUsuarios");
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUsuarioDto registro)
        {
            try
            {
                if (string.IsNullOrEmpty(registro.clave))
                {
                    ModelState.Remove("clave");
                }

                if (ModelState.IsValid)
                {
                    var usuario = __repositoryUsuario.GetxId(registro.id).Result;
                    if (usuario == null) return RedirectToAction("CreateUser", "Administrador"); ;

                    //vamos a realizar una verificacion adiciona, ... no se puede registrar
                    //una cedula que ya esta registrada, a excepcion de la que ya tiene este usuario
                    //que se va a editar...
                    Usuario usuarioVerificarCedula = await _r_usuario.GetxCedula(registro.cedula);
                    bool permitirEdicion = false, resultado = false;

                    if (usuarioVerificarCedula == null)
                        permitirEdicion = true;
                    else
                    {
                        if (usuario.usuario_cedula!.Equals(registro.cedula))
                            permitirEdicion = true;
                    }

                    if ( permitirEdicion )
                    {
                        usuario.usuario_nombre = string.IsNullOrEmpty(registro.nombre) ? usuario.usuario_nombre!.ToUpper() : registro.nombre.ToUpper();
                        usuario.usuario_cedula = string.IsNullOrEmpty(registro.cedula) ? usuario.usuario_cedula : registro.cedula;
                        usuario.usuario_email = string.IsNullOrEmpty(registro.email) ? usuario.usuario_email : registro.email;
                        usuario.usuario_celular = string.IsNullOrEmpty(registro.celular) ? usuario.usuario_celular : registro.celular;
                        usuario.usuario_estado = string.IsNullOrEmpty(registro.estado) ? usuario.usuario_estado : registro.estado;
                        usuario.RolId = registro.rol==0? usuario.RolId: registro.rol;

                        if (usuario.usuario_estado.Equals("activo") && usuario.usuario_nIntentos == 0)
                            usuario.usuario_nIntentos = 3;

                        resultado = this.__repositoryUsuario.Update(usuario).Result;
                    }


                    TempData["status"] = resultado ? "true" : "false";

                    //tenemos que realizar una actualizacion en la cache para que ocupe los nuevos datos.
                }
                else
                {

                    TempData["ModelErrors"] = ModelState.Values.SelectMany(v => v.Errors)
                                                          .Select(e => e.ErrorMessage)
                                                          .ToList();
                }

                return RedirectToAction("ListaUsuarios");
            }
            catch
            {
                return RedirectToAction("ListaUsuarios");
            }
        }

        [HttpPost]
        public ActionResult ResetPassw(PassUser new_pass)
        {

            if (ModelState.IsValid)
            {
                Usuario user = __repositoryUsuario.GetxId(new_pass.id).Result;

                user.usuario_passwd = new_pass.clave;
                user.usuario_passwd = _security.EncriptarPassword(user.usuario_passwd);

               var res = __repositoryUsuario.Update(user).Result;

                if (res)
                {
                    this.TempData["status"] = "true";
                    return RedirectToAction("ListaUsuarios");
                }
                else
                {
                    this.TempData["status"] = "false";
                    return RedirectToAction("ListaUsuarios");
                }
            }
            else
            {
                this.TempData["status"] = "false";
                return RedirectToAction("ListaUsuarios");
            }
        
        }


        [HttpGet]
        public ActionResult Delete(int id)
        {
            try
            {
                bool resultado = this.__repositoryUsuario.Delete(id).Result;

                if (!resultado)
                    ViewBag.ErrorMessage = "error";

                return RedirectToAction("CreateUser", "Administrador");
            }
            catch
            {
                return RedirectToAction("CreateUser", "Administrador");
            }
        }

       

    }
}
