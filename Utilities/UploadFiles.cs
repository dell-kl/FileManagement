using SIS_DIAF.Models;
using System.IO;

namespace SIS_DIAF.Utilities
{
    public enum Directorys
    {
        FileStorage,
        WebStorageRoot
    }
    public class UploadFiles
    {
        private const string EXTENSION = ".pdf";

        private readonly IConfiguration _configuration;

        public UploadFiles(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string? _root => _configuration.GetValue<string>(Directorys.FileStorage.ToString()!);
        private string? _relativeP => _configuration.GetValue<string>(Directorys.WebStorageRoot.ToString()!);
        
        public async Task<Archivo> SaveFile(FormFile file, Archivo arch)
        {
            if (file is null) return null!;

            string relative = "";
            string codReg = arch.Regimen.regimen_cod;
            string tipo_doc = arch.TipoArchivo.tipo_nombre.Replace('/', '-');
            string NombreTipoRegs = arch.TipoArchivo.TipoReg.nombre_tipo_reg;

            try
            {
                string FileName = Path.ChangeExtension(getNameStandar(codReg,tipo_doc), EXTENSION);

                if (NombreTipoRegs.Equals("GENERAL"))
                {
                    relative = Path.Combine(_relativeP!, codReg, tipo_doc);
                } 
                else
                {
                    relative = Path.Combine(_relativeP!, codReg, NombreTipoRegs, tipo_doc);

                }

                string path = Path.Combine(_root!, relative);

                createDirectory(path);

                await using FileStream fs = new(Path.Combine(path, FileName), FileMode.Create);
                await file.OpenReadStream().CopyToAsync(fs);

                return new Archivo()
                {
                    archivo_nombre = FileName,
                    archivo_ruta = Path.Combine("Storage\\",relative, FileName),
                    archivo_estado = "subido",
                    fecha_subida = DateTime.Now,
                };
            }
            catch
            {
                return null!;
            }
        }

        public string getNameStandar(string name, string id)
        {
            return name + "_"+ id;
        }

        private void createDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public string CopyFile(Archivo archivo)
{
            try
            {
                //reemplazamos Storage porque hacemos un reemplazo en el sistema de archivo, necesitamos ruta completa.
                archivo.archivo_ruta = archivo.archivo_ruta.Replace("Storage\\", _root!);
                string ArchivoTemporal =  archivo.archivo_ruta!.Replace(archivo.archivo_nombre!, archivo.archivo_guid);
                ArchivoTemporal = Path.ChangeExtension(ArchivoTemporal, EXTENSION);

                File.Copy(
                    archivo.archivo_ruta,
                    ArchivoTemporal
                );
                
                return ArchivoTemporal;
            }
            catch (Exception ex) {

                throw new Exception($"No se pudo guardar copia archivo. MENSAJE : {ex.Message}");
            }
        }

        public async Task<byte []> GetArchivo(string path)
        {
          
            string p = Path.Combine(_root!, path);

            if (!System.IO.File.Exists(p))
            {
                return null!;
            }

            var fileBytes =  await System.IO.File.ReadAllBytesAsync(p);

            return fileBytes;
        }

    }
}
