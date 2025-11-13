using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;
using SIS_DIAF.Security;
using System.Linq.Expressions;
using System.Drawing.Imaging;
using System.Text;

namespace SIS_DIAF.Repositorios
{
    public class RUsuario : IRepositorio<Usuario>, ILogin
    {
        private SistemaDiafContext _contex;
        private IConfiguration _config;
        private EncriptacionPass _security;
        public RUsuario(
            SistemaDiafContext contex, 
            IConfiguration config,
            EncriptacionPass security)
        {
            _contex = contex;
            _config = config;
            _security = security;
        }

        public async Task<Usuario> BuscarXRol(int idRol)
        {
            return await _contex.Usuario.
                Where(u => u.RolId.Equals(idRol))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Create(Usuario x)
        {
            await _contex.Usuario.AddAsync(x);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(long id)
        {
           var user =  await GetxId(id);

            if (user != null) { 
                 _contex.Usuario.Remove(user);
                return await _contex.SaveChangesAsync()>0;
            }
            return false;
        }

        public async Task<Usuario> GetxId(long id)
        {   
            var u = await _contex.Usuario.FindAsync(id);
            return u!;
        }

        public async Task<ICollection<Usuario>> Listar()
        {
            try
            {

                ICollection<Usuario> registros = await this._contex.Usuario
                    .Include(s => s.Sucursal)
                    .Include(r=>r.Rol).ToListAsync();
                return registros;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public async Task<bool> Update(Usuario x)
        {
            if (x != null)
            {
                 _contex.Usuario.Update(x);
                 return await _contex.SaveChangesAsync()>0;
            }

            return false;
        }

        public async Task<Usuario> GetxCedula(string ci)
        {
            var usuario = await _contex.Usuario
                .Include(r => r.Rol)
                .Where<Usuario>(
                u=> u.usuario_cedula!.Equals(ci) && u.usuario_estado.ToLower().Equals("Activo"))
                .FirstOrDefaultAsync<Usuario>();

            return usuario!;
        }

        public async Task<ICollection<Usuario>> GetXCoincidencia(string entrada)
        {
            return await _contex.Usuario
                .Where(n => n.usuario_nombre!.ToLower().Contains(entrada.ToLower()))
                .ToListAsync();

        }

        public async Task<PerfilUsuarioDto> Filtrar(LoginDto t)
        {
            try
            {
                string textoFrontend = "";

                var resultado = await this._contex.Usuario
                    .Where(u => u.usuario_cedula!.Equals(t.usuario))
                    .Include(r => r.Rol)
                    .ToListAsync();

                if ( !resultado.IsNullOrEmpty() )
                {
                    if ( resultado.First().usuario_nIntentos == 0 )
                    {
                        return new PerfilUsuarioDto()
                        {
                            mensaje = "cuenta bloqueada, acercate a TI",
                            tipo = "Cuenta Perfil Bloqueado"
                        };
                    }

                    string passwdTextPlain = resultado.First().usuario_passwd!;
                    //vamos a desencriptar la contrasena guardada... 
                    passwdTextPlain = _security.DesencriptarPassword(passwdTextPlain); 

                    //vamos a realizar la comparacion.
                    if ( passwdTextPlain.Equals(t.password) )
                        return new PerfilUsuarioDto() { 
                            id = resultado.First().usuario_id,
                            rol = resultado.First().Rol.rol_descripcion,
                            nombre = resultado.First().usuario_nombre!,
                            cedula = resultado.First().usuario_cedula!,
                            estado = resultado.First().usuario_estado,
                            tipo = "sesion_correcta"
                        };

                    //en este punto vamos a reducir el numero de intentos... porque no ha podido pasar correctamente
                    //las validaciones anteriores.
                    resultado.First().usuario_nIntentos -= (resultado.First().usuario_nIntentos > 0) ? 1 : 0;
                    resultado.First().usuario_estado = (resultado.First().usuario_nIntentos == 0) ? "inactivo" : "activo";

                    _contex.Usuario.Update(resultado.First());
                    await _contex.SaveChangesAsync();

                    //System.Text.Encoding.UTF8.GetString(System.Text.Encoding.Default.GetBytes(textoFrontend)),
                    return new PerfilUsuarioDto()
                    {
                        mensaje = $"datos incorrectos, tienes {resultado.First().usuario_nIntentos} intentos disponibles"
                    };

                }

                return new PerfilUsuarioDto()
                {
                    mensaje = "datos incorrectos"
                };

            }
            catch (Exception ex)
            {
                return new PerfilUsuarioDto()
                {
                    mensaje = "error modulo",
                    tipo = "Error modulo seguridad"
                };
            }
        }

        public Task<Usuario> GetxCod(string c)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Usuario>> filtro(int tipo)
        {
            try
            {
                int valor = _config.GetValue<int>("Sucursal");

                //con este codigo de aqui filtraba por sucursal los usuarios de logistica.
                /*return await this._contex.Usuario.Where(
                    usuario => usuario.RolId.Equals(tipo) && usuario.SucursalId.Equals(valor)
                ).ToListAsync();*/

                return await this._contex.Usuario
                    .Include(n => n.Sucursal)
                    .Where(
                    usuario => usuario.RolId.Equals(tipo) 
                ).ToListAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
            
 

        public async Task<ICollection<Usuario>> obtenerUsuarioResponsables(string tipo)
        {
            var datos = await _contex.Rol
                .Where(r => r.rol_descripcion.Equals(tipo) )
                .Select(e => e.Usuarios )
                .ToListAsync();

            return datos.First();
        }
    }
}
