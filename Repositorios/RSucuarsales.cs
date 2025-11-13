
using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;
using NuGet.Protocol;

namespace SIS_DIAF.Repositorios
{
    public class RSucuarsales : IRepositorio<Sucursal>
    {
        private SistemaDiafContext _contex;

        public RSucuarsales(SistemaDiafContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> Create(Sucursal x)
        {
            _contex.Sucursales.Add(x);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var s = await GetxId(id);
            if (s == null)
            {
                return false;
            }

            _contex.Sucursales.Remove(s);
            return await _contex.SaveChangesAsync() > 0;

        }

        public async Task<Sucursal> GetxId(long id, string c = "sin_data")
        {
            var s = await _contex.Sucursales.FindAsync(id);
            return s!;
        }

        public async Task<ICollection<Sucursal>> Listar()
        {
            return await _contex.Sucursales.Include(u => u.Usuarios).ToListAsync<Sucursal>();
        }

        public async Task<bool> Update(Sucursal x)
        {
            if (x == null)
            {
                return false;
            }

            _contex.Sucursales.Update(x);
            return await _contex.SaveChangesAsync() > 0;
        }

       
        public async Task<Sucursal> GetxId(long id)
        {
            var sucursal = await _contex.Sucursales.Include(s => s.Usuarios)
                .Where(s => s.sucursal_id! == id).FirstOrDefaultAsync();
            return sucursal!;
        }

        
        public async Task<Sucursal> GetxCod(string c)
        {
            var sucursal =  await _contex.Sucursales.Include(s => s.Usuarios)
                .Where(s => s.sucursal_nombre!.Equals(c)).FirstOrDefaultAsync();
            return sucursal!;
        }
    }
}
