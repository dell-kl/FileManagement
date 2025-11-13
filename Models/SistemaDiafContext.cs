using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SIS_DIAF.Models
{
    public class SistemaDiafContext : DbContext, IDataProtectionKeyContext
    {
        public SistemaDiafContext(DbContextOptions<SistemaDiafContext> options) : base(options) {  }

        public DbSet<ServicioCorreo> ServicioCorreo { set; get; }
        public DbSet<Usuario> Usuario { set; get; } 

        public DbSet<Rol> Rol { set; get;  }
        
        public DbSet<Archivo> Archivos { set; get; }

        public DbSet<Sucursal> Sucursales { set; get; }

        public DbSet<TipoArchivo> TipoArchivos { set; get; }

        public DbSet<Regimen> Regimens { set; get; }

        public DbSet<TipoReg> TipoRegs { set; get; }

        public DbSet<Historico> Historico { set; get; }

        public DbSet<ResponsableRegimen> ResponsableRegimens { set; get; }

        public DbSet<DataProtectionKey> DataProtectionKeys { set; get; }

    }
}
