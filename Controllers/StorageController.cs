using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.Utilities;

namespace SIS_DIAF.Controllers
{
    [Route("[controller]")]
    public class StorageController : Controller
    {
        private UploadFiles _ser_files;
        public StorageController(UploadFiles ser_files) { 
            _ser_files = ser_files;
        }

        [HttpGet("{*path}")]
        public async Task<IActionResult> mostrarArchivo(string path) {

            var encodedFileName = Uri.EscapeDataString(path);
            Response.Headers.Add("Content-Disposition", $"inline; filename={encodedFileName}");
            return File(await _ser_files.GetArchivo(path), "application/pdf");
        }

    }
}
