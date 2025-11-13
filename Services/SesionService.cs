using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using SIS_DIAF.DTO;

namespace SIS_DIAF.Services
{
    public class SesionService : ISesion 
    {

        private readonly IHttpContextAccessor __context;
        private readonly HttpContext solicitud;
        public SesionService(IHttpContextAccessor context)
        {
            this.__context = context;
            this.solicitud = this.__context.HttpContext;
        }

        string ISesion.verificarAutenticacion()
        {
            ClaimsPrincipal inicioSesion = this.solicitud.User;

            if (inicioSesion.Identity.IsAuthenticated)
            {
                var rol = inicioSesion.Claims.ToList()[1].Value.ToLower();

                return rol;
            }

            return null;
        }

        int ISesion.retornarIdentificadorUsuario() => int.Parse(this.solicitud.User.Claims.ToList()[0].Value);

        async void ISesion.autenticacion(PerfilUsuarioDto registro, LoginDto formulario)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("UsuarioID", registro.id.ToString()),
                new Claim(ClaimTypes.Role, registro.rol),
                new Claim("Cedula", registro.cedula),
                new Claim("Nombres", registro.nombre)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = formulario.AIniciadoSesion
            };

            await this.solicitud.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity), properties);
        }

    }
}
