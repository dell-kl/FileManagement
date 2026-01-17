using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.DTO;
using SIS_DIAF.DTO.DTOVEIW;
using SIS_DIAF.Models;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Services;
using SIS_DIAF.Utilities;

namespace SIS_DIAF.Controllers
{
   
    public class UploadController : Controller
    {
        private RArchivo _serv_arch;
        private RRegimen _serv_regimen;
        private const long MEGA_BYTES = 1204 * 1024;
        private const long MAX_SIZE = MEGA_BYTES * 3; //3MB
        private int ArchivosPermitidos = 1;
        private List<string> erros = new();
        private string? mesajeExito;
        private ISesion _sesion_service;

        public int identificador = 0;

        public UploadController(RArchivo serv_arch, RRegimen serv_regimen, ISesion sesionService)
        {
            _serv_regimen = serv_regimen;
            _serv_arch = serv_arch;
            _sesion_service = sesionService;


            //vamos a tener que acceder al identificador del cliente. 
            this.identificador = _sesion_service.retornarIdentificadorUsuario();
        }
        // GET: UploadContoller

        [Authorize(Roles = "finanzas,logistica administrador,jurídico,logística")]
        public async Task<ActionResult> ObtenerArchivosResponsable(string cod)
        {
            List<Archivo> archivos = new List<Archivo>();

            if (string.IsNullOrWhiteSpace(cod))
            {
                if (User.IsInRole("logística"))
                    return RedirectToAction("Index", "Logistica");
                else 
                    return RedirectToAction("Index", "Regimens");
            }

            try
            {
                ViewBag.Exito = TempData["succes"]; 
                ViewBag.Errores = TempData["error"];
            }
            catch (RuntimeBinderException ex)
            {
                ViewBag.Exito = null; ViewBag.Errores = null;
            }

            if ( User.IsInRole("finanzas") )
                archivos = await _serv_regimen.GetArchivosFinanzas(cod, 1);

            if ( User.IsInRole("logistica administrador") )
                archivos = await _serv_regimen.GetArchivos(cod, 5);

            if ( User.IsInRole("logística") )
                archivos = (List<Archivo>)await _serv_regimen.GetArchivosLogistica(cod);
                ICollection<Archivo> listaArch = new List<Archivo>();
                
                if ( archivos.Any() )
                    //la posicion de estos archivos deben encontrarse de esta manera...
                    listaArch = new List<Archivo>() {
                        await _serv_arch.GetxId(9,cod),
                        await _serv_arch.GetxId(14,cod),
                        await _serv_arch.GetxId(16,cod),
                        await _serv_arch.GetxId(17,cod),
                    };
                    
                ViewBag.archivLogistica = listaArch;

            if (User.IsInRole("jurídico"))
                archivos = await _serv_regimen.GetArchivosJuridico(cod, 1);    

            var lista = Ordenar<Archivo>.OrdearPrioridadArchivos(archivos);

            ViewBag.archivos = new DtoViewRegimen(
                codigo: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_cod : "sin_codigo",
                guid: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_guid : "sin_guid",
                archivos: lista
            );

            return View("Index");
        }


        [Authorize(Roles = "logistica administrador,administrador,responsable,supervisor,usuario final")]
        public async Task<ActionResult> obtenerArchivosLogistica(string cod)
        {
            if (string.IsNullOrWhiteSpace(cod))
            {
                if(User.IsInRole("logistica administrador"))
                    return RedirectToAction("Asignaciones", "LogisticaAdmin");
                else
                    return RedirectToAction("Index", "Regimens");
            }

            List<Archivo> archivos = (List<Archivo>) await _serv_regimen.GetArchivosLogistica(cod);

            var lista = Ordenar<Archivo>.OrdearPrioridadArchivos(archivos);

            ViewBag.archivos = new DtoViewRegimen(
                codigo: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_cod : "sin_codigo",
                guid: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_guid : "sin_guid",
                archivos: lista
            );

            return View("Index");
        }


        [Authorize(Roles = "administrador,responsable,supervisor,usuario final")]
        public async Task<ActionResult> Index(string cod)
        {
            if (string.IsNullOrWhiteSpace(cod))
                return RedirectToAction("Index", "Regimens");

            try
            {
                ViewBag.Exito = TempData["succes"];
                ViewBag.Errores = TempData["error"];
            }
            catch (RuntimeBinderException ex)
            {
                ViewBag.Exito = null; ViewBag.Errores = null;
            }

            //listado de los archivos del regimen General.
            List<Archivo> archivos = await _serv_regimen.GetArchivos(cod, 1);

            if ( archivos.Count() > 0 )
            {

                //informacion para la parte del proceso de logistica que se mostrara en el regimen general.
                //para mostrar le progreso de los archiovs Proforma, Cuadro Comparativo, Cotizacion.
                ViewBag.archivLogistica = await _serv_regimen.GetRegimenArchivosLogistica(cod, 5);
                //para saber de que tipo de es el proceso Bien, Mantenimiento o Servicio que eligio para este regimen
                ViewBag.tipoSubArchivos = await _serv_regimen.obtenerInformacionSubArchivosRegimen(cod);

            }

            var lista = Ordenar<Archivo>.OrdearPrioridadArchivos(archivos);

            ViewBag.archivos = new DtoViewRegimen(
                codigo: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_cod : "sin_codigo",
                guid: (archivos.Count > 0) ? archivos[^1].Regimen.regimen_guid : "sin_guid",
                archivos: lista
            );

            return View();
        }


        // GET: UploadContoller/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: UploadContoller/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: UploadContoller/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UploadContoller/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        [Authorize(Roles = "responsable,logística,logistica administrador,finanzas,jurídico,administrador")]
        [HttpPost]
        public async Task<IActionResult> GuardarArchivo(RequestArchivos req, string Perfiltipo = "")
        {
            FormFile arch = null!;
            erros.Clear();

            int idTipoRegimen = (int)(
                await _serv_arch.GetxId(req.id_archivo)
            ).TipoArchivo.TipoReg.tiporeg_id;

            if (req.file != null)
            {

                if (req.file.Count > ArchivosPermitidos)
                {
                    //erros[0] = "Solo se permite un archivo";
                    erros.Add("Solo se permite un archivo");
                }
                else
                {
                    arch = (FormFile)req.file[0];

                    if (GetSizeMB(arch!.Length) > MAX_SIZE)
                    {
                        //erros[0] = "Tamaño maximo 3MB";
                        erros.Add("Tañamo maximo 3MB");
                    }
                    else
                    {

                        if (Path.GetExtension(arch.FileName) != ".pdf")
                        {
                            //erros[0] = "Solo se acepta formato PDF";
                            erros.Add("Solo se acepta formato PDF");

                        }
                        else
                        {
                            var rest = await _serv_arch?.Upload(arch, req.id_archivo, req.guid_a, req.tipoNombreArchivo)!;

                            if (rest)
                            {

                                TempData["succes"] = true;
                            }
                            else
                            {
                                //erros[0] = "Se produjo un error al cargar el archivo";
                                erros.Add("Se produjo un error al cargar el archivo");
                            }
                        }

                    }
                }

            }
            else
            {
                //erros[0] = "Primero seleccione un archivo";
                erros.Add("Primero seleccione un archivo");
            }

            TempData["error"] = erros;

            string rutaFinal = "/Upload?cod=" + req.guid_r + "&tip_reg=" + idTipoRegimen + (!Perfiltipo.Equals("") ? $"&tipo={Perfiltipo}" : "");

            if ( User.IsInRole("finanzas") || User.IsInRole("logistica administrador") || User.IsInRole("logística") || User.IsInRole("jurídico"))
                rutaFinal = "/Upload/ObtenerArchivosResponsable?cod=" + req.guid_r;
             
            return Redirect(rutaFinal);
            //return RedirectToAction("Index", "Upload?cod="+req.guid_a);
        }

        //Transforma de bytes a megabytes
        private float GetSizeMB(long bytes)
        {
            return (float)bytes / MEGA_BYTES;
        }

        // POST: UploadContoller/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UploadContoller/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        public ActionResult MostrarVistaAdminArchivos()
        {

            return View();
        }

        // POST: UploadContoller/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
