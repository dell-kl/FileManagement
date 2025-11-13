using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.Models;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Security;
using SIS_DIAF.Services;
using SIS_DIAF.Utilities;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace SIS_DIAF.Controllers
{
    [Authorize(Roles = "logística")]
    public class LogisticaController : Controller
    {
        private IConfiguration _configuration;
        private ISesion _sesion;
        private RRegimen _serv_regimen;
        private RTipoArchivo _serv_tipoArchivo;
        private RArchivo _serv_archivo;
        private RResponsableRegimen _serv_responsableRegimen;
        private EncriptacionPass _security;
        private GestionarEncriptado _gestionarEncriptado;
        private int idUsuarioSesion;

        public LogisticaController(IConfiguration configuration, 
            ISesion sesion, RRegimen serv_regimen, 
            RTipoArchivo servTipoArchivo, RArchivo srv_archivo, 
            RResponsableRegimen serv_responsableRegimen, 
            EncriptacionPass security, GestionarEncriptado gestionarEncriptado) { 
            _configuration = configuration;
            _sesion = sesion;
            _serv_regimen = serv_regimen;
            _serv_tipoArchivo = servTipoArchivo;
            _serv_archivo = srv_archivo;
            _serv_responsableRegimen = serv_responsableRegimen;
            _security = security;
            _gestionarEncriptado = gestionarEncriptado;

            idUsuarioSesion = _sesion.retornarIdentificadorUsuario();
        }

        public async Task<IActionResult> Index(
            string pagina, 
            string tipo, 
            string direccion, 
            string entrada, 
            bool procesosEnCurso = false)
        {

            //vamos a encriptar los respectivos datos. 
            Dictionary<string, string> datosEncripts = new Dictionary<string, string>();
            datosEncripts.Add("pagina", pagina);
            datosEncripts.Add("tipo", tipo);
            datosEncripts.Add("direccion", direccion);
            datosEncripts.Add("entrada", entrada);
            datosEncripts = _gestionarEncriptado.SeguridadEncriptado(datosEncripts);

            if (datosEncripts == null)
                return RedirectToAction("Index");

            //vamos a setear los respectivos valores iniciales. 
            TempData["tipo"] = datosEncripts["tipo"];
            TempData["entrada"] = datosEncripts["entrada"];
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];

            ICollection<Regimen> regimenOtorgado = new List<Regimen>();
            Dictionary<string, object> datos = new Dictionary<string, object>();
            
            if (int.Parse(TempData["tipo"]!.ToString()!) == 0 && TempData["entrada"]!.ToString()!.Equals("vacio"))
                regimenOtorgado = await _serv_regimen.Listar(idUsuarioSesion);
            else
                regimenOtorgado = await Filtrar(int.Parse(TempData["tipo"]!.ToString()!), TempData["entrada"]!.ToString()!, idUsuarioSesion);

            //configurar la parte de la paginacion que se va a realizar. 
            if (direccion.Equals("siguiente") || direccion.Equals("inicio"))
                datos = Ordenar<Regimen>.Siguiente(int.Parse(pagina), regimenOtorgado);
            else
                datos = Ordenar<Regimen>.Atras(int.Parse(pagina), regimenOtorgado);

            //generar algunas configuraciones ... 
            TempData["b-back"] = datos["b-back"].ToString();
            TempData["b-s-back"] = datos["b-s-back"].ToString();

            TempData["b-next"] = datos["b-next"].ToString();
            TempData["b-s-next"] = datos["b-s-next"].ToString();

            TempData["pagina"] = _security.EncriptrarUrl( datos["pagina"].ToString()! );

            //vamos a traernos los procesos que fueron asignados y estan en proceso. 
            ViewBag.regimens = (ICollection<Regimen>)datos["info"];

            //vamos a tener que volver a realizar la encriptacion de los datos. 
            //volveremos a encriptar la informacion... 
            //datosEncripts = _gestionarEncriptado.encriptarDatos(datosEncripts);

            /* Volver a setear el encriptado en nuestras variables. */            
            TempData["tipo"] = _security.EncriptrarUrl(datosEncripts["tipo"]);
            TempData["entrada"] = _security.EncriptrarUrl(datosEncripts["entrada"]);

            pagina = _security.EncriptrarUrl(datosEncripts["pagina"]);
            direccion = _security.EncriptrarUrl(datosEncripts["direccion"]);

            ViewData["LanzarLoading"] = "d-none";

            return View();
        }

        [HttpPost]
        public ActionResult FiltrarListaRegimenes(string tipo, string entrada)
        {
            tipo = _security.EncriptrarUrl(tipo);
            entrada = _security.EncriptrarUrl(entrada);

            return RedirectToAction("Index", "Logistica", new { tipo = tipo, entrada = entrada });
        }

        public async Task<ICollection<Regimen>> Filtrar(int tipo, string entrada, int idUsuario = 0)
        {
            ICollection<Regimen> datos = new List<Regimen>();

            switch (tipo)
            {
                case 1:

                    datos = await _serv_regimen.ListXDescripcionLogistic(entrada, "objetivo", idUsuario);
                    
                    break;

                case 2:

                    //vamos a buscar por el tipo de proceso de logistica que tenga asignado,
                    //ya sea por un BIEN, MANTENIMIENTO, SERVICIO.... 
                    datos = await _serv_regimen.ListXDescripcionLogistic(entrada, "tipo", idUsuario);

                    break;
            }

            return datos;
        }


        [HttpGet]
        public async Task<IActionResult> asignarProcesoTipo(string idRegimen, string tipo, string codigo)
        {
            //traer informacion del regimen con el cual se esta manejando.
            Regimen regimenInfo = await _serv_regimen.GetxCod(codigo);

            var responsable = await _serv_responsableRegimen.searchByCodeRegimen(regimenInfo.regimen_guid, idUsuarioSesion);

            if (responsable != null)
            {
                //cambiar el estado de nuestro responsableRegimen.S
                bool estado = await _serv_responsableRegimen.cambiarEstadoResponsableRegimen(responsable);

                if (estado)
                {
                    //vamos a tener que hacer la creacion de los respectivos archivos.
                    string carpetaRuta = creacionCarpeta($"{_configuration.GetValue<string>("WebStorageRoot")}/{codigo}/{tipo.ToUpper()}");    

                    //dentro de esta ruta recien creada vamos a crear otros archivos adicionales.
                    int idTipoRegs = (tipo.Equals("mantenimiento")) ? 4 : (tipo.Equals("bienes")) ? 3 : 2;

                    //obtenemos los tipos de archvios que son para guardarlo en nuestra carpeta recien creada
                    ICollection<string> registros = await _serv_tipoArchivo.GetxTipoRegs(idTipoRegs);

                    foreach (var n in registros) {
                        string rutaCompleta = $"{carpetaRuta}/{n}";
                        if ( !Directory.Exists(rutaCompleta) )
                        {
                            Directory.CreateDirectory(rutaCompleta);
                        }
                    }

                    ICollection<TipoArchivo> registroTipoArchivo = await _serv_tipoArchivo.GetxTipoRegs(idTipoRegs, "tipoArchivos");

                    foreach (var item in registroTipoArchivo)
                    {                
                        Archivo crearArchivo = new Archivo() { 
                            archivo_guid = Guid.NewGuid().ToString(),
                            archivo_estado = item.tip_prioridad==1?"pendiente" : "vacio",
                            TipoArchivoId = item.tipoarchivo_id,
                            RegimenId = regimenInfo.regimen_id
                        };

                        //vamos a tenr que registrar a continuacion la parte de los archivos de los de logistica.
                        await _serv_archivo.Create(crearArchivo);
                    }

                    return RedirectToAction("Index", "Logistica"); 
                }
            }

            return Redirect("/Index?estado=error");
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
