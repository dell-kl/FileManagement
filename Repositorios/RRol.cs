
using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;
using Microsoft.IdentityModel.Tokens;

namespace SIS_DIAF.Repositorios
{
    public class RRol : IRepositorio<Rol>
    {
        private SistemaDiafContext _contex;

        public RRol(SistemaDiafContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> Create(Rol x)
        {
            var resultado = this._contex.Rol.Where(r => r.rol_descripcion.Equals(x.rol_descripcion)).ToListAsync().Result;

            if ( !resultado.Any() )
                _contex.Rol.Add(x);

            return await _contex.SaveChangesAsync()>0;
        }

        public async Task<bool> Delete(long id)
        {
            var r = await GetxId(id);
            if (r != null) { 
                _contex.Remove(r);
                return await _contex.SaveChangesAsync() > 0;
            }
            return false;
        }

        public async Task<Rol> GetxId(long id)
        {
            var r = await _contex.Rol.FindAsync(id);
            return r!;
        }

        public async Task<ICollection<Rol>> Listar()
        {
            try
            {
                ICollection<Rol> listado = await this._contex.Rol.ToListAsync();

                return listado;
            }   
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> Update(Rol x)
        {
            if(x != null)
            {
                var resultado = this._contex.Rol.Where(r => r.rol_descripcion.Equals(x.rol_descripcion)).ToListAsync().Result;

                /*
                si no hay nada en "resultado" es porque no se encontro ningun registro.
                si hay un registro, comparamos los id del Rol "x" y de mi resultado para 
                ver si estan colocando nuevamente el nombre de un rol ya existente... 
                */
                if (!resultado.Any() || resultado.First().rol_id.Equals(x.rol_id))
                    _contex.Rol.Update(x);

                return await _contex.SaveChangesAsync() > 0;
            }
            return false;
        }

        public Task<Rol> GetxCod(string c)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<Rol>> ListarGuid(string guid)
        {
            throw new NotImplementedException();
        }
    }
}
