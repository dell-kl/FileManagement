using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;
using SIS_DIAF.Utilities;

namespace SIS_DIAF.Repositorios
{
    public enum Directorys
    {
        FileStorage,
        WebStorageRoot
    }
    public class RHistorico 
    {
        private readonly SistemaDiafContext _context;
        private readonly IConfiguration _configuration;

        private string _root => _configuration.GetValue<string>(Directorys.FileStorage.ToString()!);

        public RHistorico(SistemaDiafContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        } 

        public async Task<bool> Create(Historico x)
        {
            try
            {
                x.historico_archivoRuta = x.historico_archivoRuta!.Replace(_root, "Storage\\");

                await _context.Historico.AddAsync(x);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception($"[!] Error en la insercion del registro. MENSAJE: {ex.Message}");
            }

        }

        public async Task<bool> Delete(long id)
        {
            try
            {
                Historico? registro = await _context.Historico.Where(n => n.historico_id.Equals(id)).FirstOrDefaultAsync();

                if (registro != null)
                    _context.Historico.Remove(registro);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"[!] No pudo eliminarse el registro. MENSAJE : {ex.Message}");
            }
        }

        public async Task<Historico> GetxCod(string c)
        {
            throw new NotImplementedException();
        }

        public Task<Historico> GetxId(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Historico>> Listar()
        {
            try
            {
                return await _context.Historico
                    .ToListAsync();
            }
            catch (Exception ex) {
                throw new Exception($"[!] No se pudo retornar los registros. MENSAJE : {ex.Message}");
            }
        }

        public async Task<ICollection<Historico>> filtro(string code)
        {
            var histo = await _context.Historico.Where<Historico>(
                h => h.historico_guidRegimen!.Equals(code)
                ).ToListAsync();
            return histo!;
        }


        public Task<bool> Update(Historico x)
        {
            throw new NotImplementedException();
        }
    }
}
