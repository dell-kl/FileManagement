using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;

namespace SIS_DIAF.Repositorios
{
    public class RTipoArchivo : IRepositorio<TipoArchivo>
    {
        private SistemaDiafContext _contex;
        public RTipoArchivo(SistemaDiafContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> Create(TipoArchivo x)
        {
            if (x == null)
            {
                return false;
            }
            _contex.TipoArchivos.Add(x);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var tip = await GetxId(id);
            if (tip == null)
            {
                return false;
            }
            _contex.TipoArchivos.Remove(tip);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<TipoArchivo> GetxId(long id)
        {
            var tip = await _contex.TipoArchivos.FindAsync(id);
            return tip!;
        }

        public async Task<ICollection<TipoArchivo>> Listar()
        {
            return await _contex.TipoArchivos.ToListAsync<TipoArchivo>();
        }

        public async Task<ICollection<TipoArchivo>> ListarXGuid(string guid)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(TipoArchivo x)
        {
            if(x == null)
            {
                return false;
            }
            _contex.TipoArchivos.Update(x);
            return await _contex.SaveChangesAsync()>0;
        }

        public Task<TipoArchivo> GetxCod(string c)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> GetxTipoRegs(int id)
        {
            var registros = await _contex.TipoArchivos
                .Where(t => t.TipoRegId.Equals(id))
                .Select(e => e.tipo_nombre)
                .ToListAsync();
            return registros;
        }

        public async Task<ICollection<TipoArchivo>> GetxTipoRegs(int id, string tipo)
        {
            var registros = await _contex.TipoArchivos
            .Where(t => t.TipoRegId.Equals(id))
            .Select(e => e)
            .ToListAsync();
            return registros;
        }

        public async Task<List<TipoArchivo>> GetFormato(long id_tipo_reg){
            var lista = await _contex.TipoArchivos.Where<TipoArchivo>(
                    ta => ta.TipoRegId == id_tipo_reg
                ).ToListAsync();
            return lista!;
        }

        /* vamos a obtener todos los registros de la tabla TipoRegs, disintot al tipo GENERAL */
        // lo vamos a usar para supervisor logistica.
        public async Task<ICollection<TipoReg>> GetTipoRegs()
        {
            return await _contex.TipoRegs
                .Where(n => !n.nombre_tipo_reg.Equals("GENERAL") && !n.nombre_tipo_reg.Equals("LOGISTICA"))
                .ToListAsync();
        }

        public async Task<TipoReg> GetTipoReg(int id)
        {
            return await _contex.TipoRegs
                .Where(n => n.tiporeg_id.Equals(id))
                .FirstAsync();
        }

        public Task<ICollection<TipoArchivo>> ListarGuid(string guid)
        {
            throw new NotImplementedException();
        }
    }
}
