using SIS_DIAF.Models;

namespace SIS_DIAF.Repositorios
{
    public interface ICorreoRepository
    {
        public Task<ServicioCorreo> obtenerDatosCorreo();
    }
}
