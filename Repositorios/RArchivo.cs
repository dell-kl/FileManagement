using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;
using SIS_DIAF.Utilities;

namespace SIS_DIAF.Repositorios
{
    public class RArchivo : IRepositorio<Archivo>
    {
        private SistemaDiafContext _contex;
        private UploadFiles _serv_upfiles;
        private RHistorico _historico;
        public RArchivo(SistemaDiafContext contex, UploadFiles serv_upfiles, RHistorico historico)
        {
            _contex = contex;
            _serv_upfiles = serv_upfiles;
            _historico = historico;
        }

        public async Task<bool> Create(Archivo? x)
        {
            if (x == null){

                return false;
            }
            await _contex.Archivos.AddAsync(x);
            return await _contex.SaveChangesAsync() > 0;

        }

        public async Task<bool> Delete(long id)
        {
            var arch = await GetxId(id);
            if (arch == null)
            {
                return false;
            }
            _contex.Archivos.Remove(arch);
            return await _contex.SaveChangesAsync() > 0;
        }
        public async Task<Archivo> GetxId(long id, string guid_reg)
        {
            var arch = await _contex.Archivos
                       .Include(r => r.Regimen)
                       .ThenInclude(r2 => r2.responsableRegimen).ThenInclude(rr => rr.usuario)
                       .Include(a => a.TipoArchivo)
                    .Where(a => a.Regimen.regimen_guid.Equals(guid_reg) && a.TipoArchivoId.Equals(id)).FirstAsync();
            return arch;
        }

        public async Task<Archivo> GetxId(long id)
        {

            var arch = await _contex.Archivos
                .Include(tp => tp.TipoArchivo)
                    .ThenInclude(tp2 => tp2.TipoReg)
                .Where<Archivo>(a => a.archivo_id == id).Include(ta => ta.TipoArchivo)
                 .Include(tr => tr.Regimen).FirstOrDefaultAsync();
            return arch!;
        }

        public async Task<Archivo> GetxCod(string c )
        {
            var arch = await _contex.Archivos.Where<Archivo>(a => a.archivo_nombre!.Equals(c))
                .FirstOrDefaultAsync();
            return arch!;
        }
        public async Task<Archivo> GetxGuid(string c)
        {
            var arch = await _contex.Archivos.Where<Archivo>(a => a.archivo_guid!.Equals(c))
                .FirstOrDefaultAsync();
            return arch!;
        }
        public async Task<ICollection<Archivo>> Listar()
        {
            return await _contex.Archivos.ToListAsync<Archivo>();
        }

        public async Task<bool> Update(Archivo x)
        {
            if (x == null)
            {
                return false;
            }
            _contex.Archivos.Update(x);
            return await _contex.SaveChangesAsync() > 0;
        }


        public async Task<bool> Upload(FormFile file, long id_archivo, string guid, string tipoNombreArchivo = "ninguno")
        {
            var arch = await GetxId(id_archivo);

            if ( !tipoNombreArchivo.Equals("ninguno") )
            {
                arch.tipoNombre = tipoNombreArchivo;
            }

            if (arch == null) return false;

            if (!arch.archivo_guid.Equals(guid)) return false;
                
            if(arch.archivo_ruta != null)
            {
                string rutaTemp = _serv_upfiles.CopyFile(arch);
                arch.archivo_guid = Guid.NewGuid().ToString();  

                //tenemos que obtener los datos del Historico para guardarlo en la base de datos.
                Historico historico = new Historico() { 
                    historico_id = 0,
                    historico_archivoRuta = rutaTemp,
                    historico_guidRegimen = arch.Regimen.regimen_guid,
                    historico_guidArchivo = arch.archivo_guid,
                    historico_tipoArchivo = arch.TipoArchivo.tipo_nombre,
                    historico_fecha = arch.fecha_subida
                };

                bool resultado = await this._historico.Create(historico);
                Console.WriteLine(resultado);
            }
            
            var rest = await _serv_upfiles.SaveFile(file, arch);

            if (rest == null) return false;
            
            arch.archivo_ruta = rest.archivo_ruta;
            arch.archivo_nombre = rest.archivo_nombre;
            arch.fecha_subida = DateTime.Now;
            arch.archivo_estado = rest.archivo_estado;

            if(await Update(arch))
            {
                return  await HabilitarNext(arch);
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> HabilitarNext(Archivo arch)
        {
            int prioridad = arch.TipoArchivo.tip_prioridad;
            string gui_reg = arch.Regimen.regimen_guid;
            int idTipoReg = (int)arch.TipoArchivo.TipoReg.tiporeg_id;
            //vamos a buscar por el ultimo nivel de prioridad para la parte de
            //estos regimenes que son los GENERAL.
            TipoArchivo numeroPrioridad = await _contex.TipoArchivos
                .Where(n => n.TipoRegId.Equals(idTipoReg))
                .OrderByDescending(p => p.tip_prioridad).FirstAsync();

            var archi  = await _contex.Archivos
                .Include(a => a.Regimen)
                .Include(a => a.TipoArchivo)
                .Where(a => a.Regimen.regimen_guid.Equals(gui_reg) && a.TipoArchivo.TipoReg.tiporeg_id.Equals(idTipoReg) && a.TipoArchivo.tip_prioridad == (!numeroPrioridad.tip_prioridad.Equals(prioridad) ? prioridad + 1 : prioridad) )
                        .FirstOrDefaultAsync<Archivo>();

            if (archi == null) return false;

            if (archi.archivo_ruta != null) return true;

            archi.archivo_estado = "pendiente";

            _contex.Archivos.Update(archi);
            return await _contex.SaveChangesAsync()>0;
        }

        public Task<ICollection<Archivo>> filtro(string tipo)
        {
            throw new NotImplementedException();
        }
    }
}
