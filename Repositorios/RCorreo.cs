using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;

namespace SIS_DIAF.Repositorios
{
    public class RCorreo : ICorreoRepository
    {
        private readonly SistemaDiafContext _context;

        public RCorreo(SistemaDiafContext context) {
            _context = context;
        }

        public async Task<ServicioCorreo> obtenerDatosCorreo() => await _context.ServicioCorreo.FirstAsync();
    }
}
