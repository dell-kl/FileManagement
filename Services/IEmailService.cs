using SIS_DIAF.DTO;
using SIS_DIAF.Models;

namespace SIS_DIAF.Services
{
    public interface IEmailService
    {
        public Task distribuirMensajeCorreo();
        public bool enviocorreo(string to, Regimen datos, string UsuarioFinal);

    }
}
