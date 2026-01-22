using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SIS_DIAF.Models;
using SIS_DIAF.Security;

namespace SIS_DIAF.Seed
{
    public class SeederDiaf
    {

        private readonly EncriptacionPass _security;
        public SeederDiaf(EncriptacionPass security)
        {
            _security = security;
        }

        public void SeederDBDiaf (SistemaDiafContext context)
        {
            context.Database.EnsureCreated ();

            if (!context.TipoRegs.Any())
            {
                var sql = File.ReadAllText("./Transact-SQL/SeedTipoReg.sql");
                context.Database.ExecuteSqlRaw(sql);
            }

            if (!context.Sucursales.Any())
            {
                var sql = File.ReadAllText("./Transact-SQL/SeedSucursales.sql");
                context.Database.ExecuteSqlRaw(sql);
            }

            if (!context.TipoArchivos.Any())
            {
                var sql = File.ReadAllText("./Transact-SQL/SeedTipoArchivo.sql");
                context.Database.ExecuteSqlRaw(sql);
            }
           
            if (!context.Rol.Any())
            {
                var sql = File.ReadAllText("./Transact-SQL/SeedRol.sql");
                context.Database.ExecuteSqlRaw(sql);
            }

            
            if (!context.ServicioCorreo.Any())
            {
                string email = "email_enviar_correos";
                string credenciales = "password";
                string host = "host_mail";

                credenciales = _security.EncriptarPassword(credenciales);
                email = _security.EncriptarDatos(email);
                host = _security.EncriptarDatos(host);

                ServicioCorreo servicioCorreo = new ServicioCorreo()    
                {
                    srvCorreo_email = email,
                    srvCorreo_host = host,
                    srvCorreo_password = credenciales,
                    srvCorreo_port = 465
                };

                context.ServicioCorreo.Add(servicioCorreo);
            }

            if (!context.Usuario.Any())
            {
                string credencialTI = "Tec$212@001";
                credencialTI = _security.EncriptarPassword(credencialTI);

                //vamos a generar un usuario general... pero con liq y no con Transact, porque debemos usar
                //la parte de seguridad de contrasena. 
                Usuario u = new Usuario()
                {
                    usuario_nombre = "SGOS. GABRIEL CARAGULLA",
                    usuario_cedula = "1003896931",
                    usuario_email = "gcaragulla@diaf.gob.ec",
                    usuario_guid = Guid.NewGuid(),
                    usuario_celular = "0985672793",
                    usuario_passwd = credencialTI,
                    usuario_estado = "activo",
                    RolId = 1,
                    SucursalId = 1,
                    usuario_nIntentos = 3
                };

                context.Usuario.Add(u);   
            }

            context.SaveChanges();
        }
    }
}
