using Microsoft.AspNetCore.Mvc;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;

namespace SIS_DIAF.Services
{
    public interface ISesion
    {
        void autenticacion(PerfilUsuarioDto registro, LoginDto formulario);

        int retornarIdentificadorUsuario();

        public string verificarAutenticacion();
    }
}
