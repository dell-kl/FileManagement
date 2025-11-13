using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using SIS_DIAF.Models;
using SIS_DIAF.Models.ModelViews;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Security;
using SIS_DIAF.Services;
using SIS_DIAF.Utilities;
using System.Globalization;

namespace SIS_DIAF.Controllers
{
    [Authorize(Roles = "logistica administrador")]
    public class LogisticaAdminController : Controller
    {

        private readonly RUsuario _usuario;
        private readonly RRegimen _regimen;
        private IResponsableRegimen _responsableRegimen;
        private readonly RResponsableRegimen _r_responsableRegimen;
        private readonly ISesion _sesion;
        private readonly RTipoArchivo _rtipoArchivo;
        private readonly IConfiguration _configuration;
        private readonly RArchivo _rArchivo;
        private readonly GestionarEncriptado _gestionarEncriptado;
        private readonly EncriptacionPass _security;
        public LogisticaAdminController(
            RUsuario usuario,
            RRegimen regimen,
            ISesion sesion,
            IResponsableRegimen responsableRegimen,
            RResponsableRegimen r_responsableRegimen,
            RTipoArchivo _r_tipoArchivo,
            IConfiguration _r_configuration,
            RArchivo _r_archivo,
            GestionarEncriptado gestionarEncriptado,
            EncriptacionPass security
        )
        {
            this._usuario = usuario;
            this._regimen = regimen;
            this._sesion = sesion;
            this._r_responsableRegimen = r_responsableRegimen;
            this._responsableRegimen = responsableRegimen;
            this._rtipoArchivo = _r_tipoArchivo;
            this._configuration = _r_configuration;
            this._rArchivo = _r_archivo;
            this._gestionarEncriptado = gestionarEncriptado;
            this._security = security;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string tipo, string pagina, string direccion, string entrada, string estado = "sin_estado")
        {
            /*
             el -> tipo, entrada <- son dos parametros que se usa en el formulario de 
             busqueda rapida ... porque buscamos por algunos filtros ...
             */

            Dictionary<string, string> datosEncripts = new Dictionary<string, string>();
            datosEncripts.Add("pagina", pagina);
            datosEncripts.Add("tipo", tipo);
            datosEncripts.Add("direccion", direccion);
            datosEncripts.Add("entrada", entrada);
            datosEncripts = _gestionarEncriptado.SeguridadEncriptado(datosEncripts);


            if (datosEncripts == null)
                return RedirectToAction("Index");

            /* Configurar las variables ya con los datos desencriptados. */
            TempData["tipo"] = datosEncripts["tipo"];
            TempData["entrada"] = datosEncripts["entrada"];
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];

            int id_AdminLogistica = this._sesion.retornarIdentificadorUsuario();
            Dictionary<string, object> datos = new Dictionary<string, object>();
            ICollection<Regimen> resultado = new List<Regimen>();

            //vamos a tener que hacer un filtrado de la informacion que se va a mostrar... lo de abajo solo es la pagina.
            if (int.Parse(TempData["tipo"]!.ToString()!) == 0 && TempData["entrada"]!.ToString()!.Equals("vacio"))
                resultado = await this._r_responsableRegimen.ArchivosRegimenAsignado(id_AdminLogistica);
            else
                resultado = await Filtrar(id_AdminLogistica, int.Parse(TempData["tipo"]!.ToString()!), TempData["entrada"]!.ToString()!);

            //gestionamos los datos con este paginador. 
            if (direccion.Equals("siguiente") || direccion.Equals("inicio"))
                datos = Ordenar<Regimen>.Siguiente(int.Parse(pagina), resultado);
            else
                datos = Ordenar<Regimen>.Atras(int.Parse(pagina), resultado);


            resultado = (ICollection<Regimen>)datos["info"];

            //traernos todos los usuarios que son parte de logistica o responsables de esa area. 
            ICollection<Usuario> datosUsuarios = await this._usuario.filtro(6); // nos vamos a filtrar por el identificador del de logistica
            ICollection<TipoReg> tipoRegs = await this._rtipoArchivo.GetTipoRegs();

            //vamos a insertarlo dentro de un modelView para poder ser representado en la vista. 
            var regimenesCompletados = new RegimenCompletadosModelView()
            {
                regimenes = resultado,
                usuarios = datosUsuarios,
                tipoRegs = tipoRegs
            };

            //este estado de aqui especificaremos si se pudo asignar correctamente a un delegado. 
            ViewBag.estado = estado;

            TempData["b-back"] = datos["b-back"].ToString();
            TempData["b-s-back"] = datos["b-s-back"].ToString();

            TempData["b-next"] = datos["b-next"].ToString();
            TempData["b-s-next"] = datos["b-s-next"].ToString();

            TempData["pagina"] = _security.EncriptrarUrl(datos["pagina"].ToString()!);

     
            //volver a seter nuevamente el encriptado realizado.
            TempData["tipo"] = _security.EncriptrarUrl(datosEncripts["tipo"]);
            TempData["entrada"] = _security.EncriptrarUrl(datosEncripts["entrada"]);
            pagina = _security.EncriptrarUrl(datosEncripts["pagina"]);
            direccion = _security.EncriptrarUrl(datosEncripts["direccion"]);

            ViewData["LanzarLoading"] = "d-none";

            return View(regimenesCompletados);
        }

        [HttpPost]
        public ActionResult FiltrarListaRegimens(string tipo, string entrada)
        {
            //vamos a encriptar nuestro filtrado. 
            tipo = _security.EncriptrarUrl(tipo);
            entrada = _security.EncriptrarUrl(entrada);
            return RedirectToAction("Index", "LogisticaAdmin", new {tipo=tipo,entrada=entrada});
        }

        [HttpPost]
        public ActionResult FiltrarlistaAsignaciones(string tipo, string entrada)
        {
            //vamos a encriptar nuestro filtrado.
            tipo = _security.EncriptrarUrl(tipo);
            entrada = _security.EncriptrarUrl(entrada);
            return RedirectToAction("Asignaciones", "LogisticaAdmin", new { tipo=tipo, entrada=entrada});
        }

        public async Task<ICollection<Regimen>> Filtrar(int idUsuario, int tipo, string entrada)
        {
            ICollection<Regimen> datos = new List<Regimen>();

            switch (tipo)
            {
                case 1:

                    datos = await _r_responsableRegimen.ArchivosRegimenAsignado(idUsuario, "objetivo", entrada);

                    break;

            }

            return datos;
        }


        [HttpGet]
        public async Task<IActionResult> Asignaciones(string pagina, string tipo, string direccion, string entrada)
        {
            //nos va a tocar encriptar los datos correspondientes. 
            Dictionary<string, string> datosEncripts = new Dictionary<string, string>();
            datosEncripts.Add("pagina", pagina);
            datosEncripts.Add("tipo", tipo);
            datosEncripts.Add("direccion", direccion);
            datosEncripts.Add("entrada", entrada);
            datosEncripts = _gestionarEncriptado.SeguridadEncriptado(datosEncripts);

            if (datosEncripts == null)
                return RedirectToAction("Asignaciones");

            //vamos a setear los respectivos datos desencriptados. 
            TempData["tipo"] = datosEncripts["tipo"];
            TempData["entrada"] = datosEncripts["entrada"];
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];

            ICollection<ResponsableRegimen> regimenes = new List<ResponsableRegimen>();
            Dictionary<string, object> datos = new Dictionary<string, object>();

            //vamos a realizar un listado de todos los RESPONSABLES LOGISTICA
            //que ya tenga uno o varios procesos asignados...
            if (int.Parse(TempData["tipo"]!.ToString()!) == 0 && TempData["entrada"]!.ToString()!.Equals("vacio"))
                regimenes = await this._regimen.Listar("logistica administrador");
            else
                regimenes = await Filtrar(int.Parse(TempData["tipo"]!.ToString()!), TempData["entrada"]!.ToString()!);

            
            //el paginador controlar la parte de presentar la informacion de 5 en 5. 
            if (direccion.Equals("siguiente") || direccion.Equals("inicio"))
                datos = Ordenar<ResponsableRegimen>.Siguiente(int.Parse(pagina), regimenes);
            else
                datos = Ordenar<ResponsableRegimen>.Atras(int.Parse(pagina), regimenes);


            TempData["b-back"] = datos["b-back"].ToString();
            TempData["b-s-back"] = datos["b-s-back"].ToString();

            TempData["b-next"] = datos["b-next"].ToString();
            TempData["b-s-next"] = datos["b-s-next"].ToString();

            TempData["pagina"] = _security.EncriptrarUrl( datos["pagina"].ToString() );

            ViewBag.regimen = (ICollection<ResponsableRegimen>)datos["info"];

            //volver a seter nuevamente el encriptado realizado.
            TempData["tipo"] = _security.EncriptrarUrl(datosEncripts["tipo"]);
            TempData["entrada"] = _security.EncriptrarUrl(datosEncripts["entrada"]);
            pagina = _security.EncriptrarUrl(datosEncripts["pagina"]);
            direccion = _security.EncriptrarUrl(datosEncripts["direccion"]);

            return View();
        }

        //opciones de filtrado. 
        public async Task<ICollection<ResponsableRegimen>> Filtrar(int tipo, string entrada, int idUsuario = 0)
        {
            ICollection<ResponsableRegimen> datos = new List<ResponsableRegimen>();

            switch (tipo)
            {
                case 1:

                    datos = await _regimen.ListxCedula(entrada, "logisticaAdmin");

                    break;

                case 2:

                    datos = await _regimen.ListXPalabraClave(entrada, "objetivo", "logisticaAdmin");

                    break;

                case 3:

                    datos = await _regimen.ListXPalabraClave(entrada, "responsable", "logisticaAdmin");

                    break;
            }

            return datos;
        }


        [HttpPost]
        public async Task<ActionResult> RegistrarResponsable(
            string GuidRegimen,
            string CodeRegimen,
            int IdUsuarioLogistica,
            int IdTipoRegimen
        )
        {
            //sacarnos la parte del id del regime.
            long id = this._regimen.getIdCurrent(GuidRegimen).Result;

            if (id != 0)
            {
                //vamos a registrar en nuestra tabla de responsable regimen.
                bool respuesta = await this._responsableRegimen.guardarResponsable(
                    id,
                    GuidRegimen,
                    IdUsuarioLogistica
                );

                /*
                 PROCESO PARA CREAR LOS ARCHIVOS DE BIEN, MANTENIMIENTO O SERVICIO
                 dependiendo de cual eligio el supervisor logistica para el responsable.
                 */

                //nos traemos el tipo
                TipoReg tipo = await this._rtipoArchivo.GetTipoReg(IdTipoRegimen);

                //vamos a tener que hacer la creacion de los respectivos archivos.
                string carpetaRuta = creacionCarpeta($"{_configuration.GetValue<string>("FileStorage")}{_configuration.GetValue<string>("WebStorageRoot")}\\{CodeRegimen}\\{tipo.nombre_tipo_reg.ToUpper()}");

                //obtenemos los tipos de archvios que son para guardarlo en nuestra carpeta recien creada
                ICollection<string> registros = await this._rtipoArchivo.GetxTipoRegs(IdTipoRegimen);

                foreach (var n in registros)
                {
                    string rutaCompleta = $"{carpetaRuta}/{n}";
                    if (!Directory.Exists(rutaCompleta))
                    {
                        Directory.CreateDirectory(rutaCompleta);
                    }
                }

                //vamos a registrar en nuestra base de datos los archivos correspondientes de la categoria especifica.

                ICollection<TipoArchivo> registroTipoArchivo = await _rtipoArchivo.GetxTipoRegs(IdTipoRegimen, "tipoArchivos");


                foreach (var item in registroTipoArchivo)
                {
                    Archivo crearArchivo = new Archivo()
                    {
                        archivo_guid = Guid.NewGuid().ToString(),
                        archivo_estado = item.tip_prioridad == 1 ? "pendiente" : "vacio",
                        TipoArchivoId = item.tipoarchivo_id,
                        RegimenId = id
                    };

                    //vamos a tenr que registrar a continuacion la parte de los archivos de los de logistica.
                    await this._rArchivo.Create(crearArchivo);
                }


                if (respuesta)
                    return RedirectToAction("Index", new { estado = "correcto" });
                else
                    return RedirectToAction("Index", new { estado = "incorrecto" });
            }
            return RedirectToAction("Index", new { estado = "incorrecto" });
        }


        public string creacionCarpeta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                //vamos a crear este directorio de ruta.
                Directory.CreateDirectory(ruta);
            }
            return ruta;
        }
    }
}
