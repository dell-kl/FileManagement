using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SelectPdf;
using SIS_DIAF.DTO;
using SIS_DIAF.Models;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Security;
using SIS_DIAF.Services;
using SIS_DIAF.Utilities;
using System.Security.Claims;

namespace SIS_DIAF.Controllers
{
    public class RegimensController : Controller
    {
        private RRegimen _serv_regimen;
        private RArchivo _serv_archivo;
        private RTipoArchivo _serv_tipoArch;
        private IRepositorio<Usuario> _serv_usuario;
        private RSucuarsales _serv_sucuarsales;
        private string _fomar_name = "RE-CEP-CGFAE";
        private RResponsableRegimen _res_responRegimen;
        private IConfiguration _configuration;
        private int idUsuarioSesion;
        private ISesion _sesion;
        private RUsuario _r_usuario;
        private EncriptacionPass _security;
        private GestionarEncriptado _gestionarEncriptado;
        private IEmailService _emailService;
        public RegimensController(RRegimen serv_regimen,
             RArchivo serv_archivo,
             RTipoArchivo serv_tipoArch,
             IRepositorio<Usuario> serv_usuario,
             RSucuarsales serv_sucuarsales,
             RResponsableRegimen res_regimen,
             IConfiguration configuration,
             ISesion sesion,
             RUsuario r_usuario,
             EncriptacionPass security,
             GestionarEncriptado gestionarEncriptado,
             IEmailService emailService
              )
        {
            _serv_regimen = serv_regimen;
            _serv_archivo = serv_archivo;
            _serv_tipoArch = serv_tipoArch;
            _serv_usuario = serv_usuario;
            _serv_sucuarsales = serv_sucuarsales;
            _res_responRegimen = res_regimen;
            _configuration = configuration;
            _sesion = sesion;
            _r_usuario = r_usuario;
            _security = security;
            _gestionarEncriptado = gestionarEncriptado;
            _emailService = emailService;
        }


        [Authorize(Roles = "administrador,responsable,supervisor,usuario final,finanzas,jurídico")]
        public async Task<IActionResult> Index(string pagina, string tipo, string direccion, string entrada, string presupuesto)
        {
            Dictionary<string, string> datosEncripts = new Dictionary<string, string>();
            datosEncripts.Add("pagina", pagina);
            datosEncripts.Add("tipo", tipo);
            datosEncripts.Add("direccion", direccion);
            datosEncripts.Add("entrada", entrada);
            datosEncripts.Add("presupuesto", presupuesto);

            datosEncripts = _gestionarEncriptado.SeguridadEncriptado(datosEncripts);

            if (datosEncripts == null)
                return RedirectToAction("Index");
            
            /*Configurar las variables...*/
            TempData["tipo"] = datosEncripts["tipo"];
            TempData["entrada"] = datosEncripts["entrada"];
            TempData["presupuesto"] = datosEncripts["presupuesto"];
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];

            /* Hacer las siguientes verificaciones. */
            idUsuarioSesion = _sesion.retornarIdentificadorUsuario();
            
            int idSucursal = _configuration.GetValue<int>("Sucursal");
            var rol = User.FindFirst(ClaimTypes.Role)?.Value;

            if ( rol.Equals("responsable") )
            {
                if (int.Parse(TempData["tipo"]!.ToString()!) == 0 && TempData["entrada"]!.Equals("vacio") && int.Parse(TempData["presupuesto"]!.ToString()!) == 0)
                    ViewBag.regimens = await _res_responRegimen.regimenResponsable(idUsuarioSesion);
                else
                    ViewBag.regimens = await Filtrar(int.Parse(TempData["tipo"]!.ToString()!), int.Parse(TempData["presupuesto"]!.ToString()!), TempData["entrada"]!.ToString()!, idUsuarioSesion);
            }
            else
            {
                if (int.Parse(TempData["tipo"]!.ToString()!)==0 && TempData["entrada"]!.Equals("vacio") && int.Parse(TempData["presupuesto"]!.ToString()!) == 0)
                    ViewBag.regimens = await _serv_regimen.Listar();
                else
                    ViewBag.regimens = await Filtrar(int.Parse(TempData["tipo"]!.ToString()!), int.Parse(TempData["presupuesto"]!.ToString()!), TempData["entrada"]!.ToString()!);
                
                ViewBag.sucursal = (Sucursal) await _serv_sucuarsales.GetxId(idSucursal);  
                ViewBag.usuariosResponsables = await _r_usuario.obtenerUsuarioResponsables("responsable");
            }


            Dictionary<string, object> datos = new Dictionary<string, object>();
            if (direccion.Equals("siguiente") || direccion.Equals("inicio"))
                datos = Ordenar<Regimen>.Siguiente(int.Parse(pagina), (ICollection<Regimen>)ViewBag.regimens);
            else
                datos = Ordenar<Regimen>.Atras(int.Parse(pagina), (ICollection<Regimen>) ViewBag.regimens);

            TempData["b-back"] = datos["b-back"].ToString();            
            TempData["b-s-back"] = datos["b-s-back"].ToString();

            TempData["b-next"] = datos["b-next"].ToString();
            TempData["b-s-next"] = datos["b-s-next"].ToString();

            TempData["pagina"] = _security.EncriptrarUrl( datos["pagina"].ToString()! );

            //volveremos a encriptar la informacion... 
            datosEncripts = _gestionarEncriptado.encriptarDatos(datosEncripts);

            /*Estos de aqui hay que mandarlos nuevamente encriptados.*/            
            TempData["tipo"] = _security.EncriptrarUrl(TempData["tipo"]!.ToString()! );
            TempData["entrada"] = _security.EncriptrarUrl(TempData["entrada"]!.ToString()! );
            TempData["presupuesto"] = _security.EncriptrarUrl(TempData["presupuesto"]!.ToString()!);

            /* Volver a setear el encriptado en nuestras variables. */
            pagina = datosEncripts["pagina"];
            direccion = datosEncripts["direccion"];

            ViewBag.regimens = (ICollection<Regimen>)datos["info"];
            ViewData["LanzarLoading"] = "d-none";

            return View();
        }

        [HttpPost]
        public ActionResult filtrarListadoRegimens(string presupuesto, string tipo, string entrada)
        {
            //vamos a filtrar los datos respectivos....
            presupuesto = (!presupuesto.IsNullOrEmpty()) ? _security.EncriptrarUrl(presupuesto) : presupuesto;
            tipo = (!tipo.IsNullOrEmpty()) ? _security.EncriptrarUrl(tipo) : tipo;
            entrada = (!entrada.IsNullOrEmpty()) ? _security.EncriptrarUrl(entrada) : entrada;

            //vamos a tener que redireccioanrlo en alguna parte. 
            return RedirectToAction("Index", "Regimens", new { tipo = tipo, entrada = entrada, presupuesto = presupuesto });
        }

        public async Task<ICollection<Regimen>> Filtrar(int tipo, int presupuesto, string entrada, int idUsuario = 0)
        {
            ICollection<Regimen> datos = new List<Regimen>();

            switch (tipo)
            {
                case 0:

                    datos = await _serv_regimen.ListxPresupuesto(presupuesto, idUsuario);

                    break;

                case 1:

                    datos = 
                        (presupuesto == 0) 
                        ?
                        await _serv_regimen.ListxCedula(entrada) 
                        :
                        await _serv_regimen.ListXCedula_Presupuesto(entrada, presupuesto);

                    break;

                case 2:

                    if (idUsuario > 0)
                        datos =
                            (presupuesto == 0 )
                            ?
                            await _serv_regimen.ListXPalabraClave(entrada, "objetivo", idUsuario)
                            :
                            await _serv_regimen.ListXPalabraOPresupuesto(entrada, "objetivo", presupuesto, idUsuario);
                    else
                        datos = (presupuesto == 0 ) 
                            ? 
                            await _serv_regimen.ListXPalabraClave(entrada, "objetivo")
                            :
                            await _serv_regimen.ListXPalabraOPresupuesto(entrada, "objetivo", presupuesto);

                    break;

                case 3:

                    datos = (presupuesto == 0 ) 
                        ? 
                        await _serv_regimen.ListXPalabraClave(entrada, "responsable") 
                        :
                        await _serv_regimen.ListXPalabraOPresupuesto(entrada, "responsable", presupuesto);

                    break;
            }

            return datos;
        }


        [Authorize(Roles = "administrador, supervisor")]
        public async Task<IActionResult> Guardar(RequesRegimen reg)
        {
            try
            {
                /*
                 * Estas validaciones son una medida de seguridad adicional ... tambien vamos adjuntar una validacion
                 * adicional dentro de nuestro front-end con ayuda de javascript. 
                 */

                if (reg == null)
                {
                    TempData["error"] = "No dejes vacios los campos al momento de intentar crear el regimen.";
                    return  RedirectToAction("Index");
                }

                if ( !reg.objetivo.IsNullOrEmpty() && reg.usuario != 0 && reg.presupuesto != 0 && reg.sucursal != 0 && reg.tipo != 0 )
                {
                    //antes de realizar todo el proceso de creacion del regimen y las respectivas asignaciones del responsable,etc.,
                    //tenemos que validar primero si existe un usuario con el perfil de "LOGISTICA ADMINISTRADOR" que tambien
                    //se le asigna dicho regimen para que suba los archivos correspondientes. 

                    //solo va a existir un Logistica Administrador, Por eso no traigo una lista Usuarios con el Rol... solo una persona.
                    Usuario usuarioLogisticaAdministador = await _r_usuario.BuscarXRol(2); // el ID 2 Logistica Administrador debe ser estatico...

                    if ( usuarioLogisticaAdministador != null)
                    {
                        string gen_cod = GenerarCod();
                        string id_guid = Guid.NewGuid().ToString();
            
                        if (await _serv_regimen.Create(new  () {
                            regimen_cod = gen_cod,
                            regimen_guid = id_guid,
                            regimen_objetivo = reg.objetivo,
                            regimen_presupuesto = reg.presupuesto
                           // UsuarioId = reg.usuario
                
                        }))
                        {
                            var ultimo = await _serv_regimen.getIdCurrent(id_guid); 
                            var listaArchivos = await _serv_tipoArch.GetFormato(reg.tipo);

                            //vamos a tener que guardar en la tabla de ResponsableRegimen.
                            if (await _res_responRegimen.guardarResponsable(
                                ultimo,
                                id_guid,
                                (int)reg.usuario,
                                "asignado"
                            ))
                            {
                                //tenemos que asignarle tambien a nuestro "logistica administrador"
                                Usuario usuarioLogisticaAdministrador = _r_usuario.filtro(2).Result.First(); // el 2 -> id logistica administrador

                                //cuando se le asigna el regimen a un responsable, tambien le vamos
                                //asignar al "supervisor logistica" para suba sus tres archivos correspondientes.
                                if (await _res_responRegimen.guardarResponsable(
                                    ultimo,
                                    id_guid,
                                    (int)usuarioLogisticaAdministrador.usuario_id,
                                    "asignado"
                                ))
                                {
                                    //registramos los Archivos del Regimen General.
                                    foreach (var item in listaArchivos)
                                    {
                                        Archivo archivoNuevo = new Archivo()
                                        {
                                            archivo_id = 0,
                                            archivo_guid = Guid.NewGuid().ToString(),
                                            archivo_estado = item.tip_prioridad == 1 ? "pendiente" : "vacio",
                                            TipoArchivoId = item.tipoarchivo_id,
                                            RegimenId = ultimo
                                        };
                                        await _serv_archivo.Create(archivoNuevo);
                                    }

                                    //registramos los Archivos de Logistica que son parte del regimen general.
                                    // 5-> id del LOGISTICA en la Tabla TipoRegs
                                    listaArchivos = await _serv_tipoArch.GetFormato(5); 

                                    foreach(var item in listaArchivos)
                                    {
                                        Archivo archivoNuevo = new Archivo()
                                        {
                                            archivo_id = 0,
                                            archivo_guid = Guid.NewGuid().ToString(),
                                            archivo_estado = item.tip_prioridad == 1 ? "pendiente" : "vacio",
                                            TipoArchivoId = item.tipoarchivo_id,
                                            RegimenId = ultimo
                                        };

                                        await _serv_archivo.Create(archivoNuevo);
                                    }

                                    //envio de correo electronico 
                                    await _emailService.distribuirMensajeCorreo();
                                }
                            }
                        }
                    }
                    else
                    {
                        TempData["error"] = "Crear primero un perfil Logistica Administrador para seguir con el proceso.";   
                    }
                }
                else
                {
                    TempData["error"] = "Completa todos los campos para crear el regimen.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                TempData["error"] = "Lo sentimos, hubo un error... intentalo nuevamente despues de un 1 minuto.";
                return RedirectToAction("Index");
            }
        }

        private string GenerarCod()
        {
            return String.Concat(_fomar_name,"-", DateTime.Now.Day,"-",
                DateTime.Now.Year,"-",DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
        }

        public async Task<ActionResult> GenerarPdfReporte(string cod)
        {
            Regimen regimen = await _serv_regimen.GetRegimenArchivos(cod);

            var nePdf = new HtmlPdfGenerator();
            nePdf.agregarDatosPdf(regimen);
            string htmlPdf = nePdf.generHtml();

            HtmlToPdf converter = new HtmlToPdf();
            PdfDocument pdfDocument = converter.ConvertHtmlString(htmlPdf);

            using (var memoryStream = new MemoryStream())
            {
                pdfDocument.Save(memoryStream);

                pdfDocument.Close();
               
                byte[] pdfBytes = memoryStream.ToArray();

                return File(pdfBytes, "application/pdf");
            }
        }

        public async Task<ActionResult> Reporteria()
        {
            return View();
        }

    }
}
