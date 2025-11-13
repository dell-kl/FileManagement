using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.Models;
using System.Drawing;

namespace SIS_DIAF.Repositorios
{
    public class RResponsableRegimen : IResponsableRegimen
    {
        private readonly SistemaDiafContext __context;
        private readonly IConfiguration _config;
        private int sucursalId;
        public RResponsableRegimen(SistemaDiafContext context, IConfiguration config)
        {
            __context = context;
            _config = config;

            this.sucursalId = _config.GetValue<int>("Sucursal");
        }

        public async Task<bool> cambiarEstadoResponsableRegimen(ResponsableRegimen _resp_regimen)
        {
            _resp_regimen.responsableReg_estado = "asignado";
            __context.ResponsableRegimens.Update(_resp_regimen);

            return await __context.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<Regimen>> ArchivosRegimenAsignado(int idUsuarioLogisticaAdmin, string tipo, String entrada)
        {
            ICollection<Regimen> regimenes = new List<Regimen>();

            if ( tipo.Equals("objetivo") )
            {
                //filtrar por los regimenes que no se ha asignado todavia un usuario o que no se han subido todos sus archivos. 
            
                regimenes = await __context.ResponsableRegimens
                    .Include(u => u.usuario)
                        .ThenInclude(r => r.Rol)
                    .Where(r => 
                            r.Usuariousuario_id.Equals((long)idUsuarioLogisticaAdmin)
                            && 
                            r.regimen.regimen_objetivo.ToLower().Contains(entrada.ToLower())
                    )
                      .Select(n => n.regimen)
                      .ToListAsync();

                ICollection<Regimen> sinResponsable = new List<Regimen>();
                foreach(var i in regimenes)
                {

                    var responsable = await __context.ResponsableRegimens
                        .Where(n => n.usuario.Rol.rol_id.Equals(6) && n.responsableReg_codigoRegimen.Equals(i.regimen_guid))
                        .FirstOrDefaultAsync();

                    if (responsable == null)
                    {
                        ICollection<Archivo> archivos = await __context.Archivos
                        .Where(n => n.Regimen.regimen_id.Equals(i.regimen_id) && n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("LOGISTICA") )
                        .ToListAsync();
                        i.Archivos = archivos;
                        sinResponsable.Add(i);
                    }

                }
                regimenes = sinResponsable;
            }

            return regimenes;
        }

        public async Task<ICollection<Regimen>> ArchivosRegimenAsignado(int idUsuarioLogisticaAdmin)
        {
            //traemos los regimenes que fueron asignado tambien a supervisor logistica
            //pero que no tenga sus tres archivos subidos y no tenga aun un responsable
            //logistica asignado.
            ICollection<Regimen> regimenes = await __context.ResponsableRegimens
                .Where(r => r.Usuariousuario_id.Equals((long) idUsuarioLogisticaAdmin))
                .Select(n => n.regimen)
                .ToListAsync();

            //evaluamos con el id -> 8 <- porque pertenece al identificador del registro LOGISTICA
            foreach(var regimen in regimenes)
            {
                ICollection<Archivo> subArchivos = await __context.Archivos
                    .Where(n => n.Regimen.regimen_id.Equals(regimen.regimen_id)
                    && (
                        n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("SERVICIOS") ||
                        n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("BIENES") ||
                        n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("MANTENIMIENTO")
                    )).ToListAsync();


                ICollection<Archivo> archivos = await __context.Archivos
                    .Where(n => n.Regimen.regimen_id.Equals(regimen.regimen_id) && n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("LOGISTICA") && (n.archivo_ruta == null || subArchivos.IsNullOrEmpty() ))
                    .ToListAsync(); 

                regimen.Archivos = archivos;
            }

            return regimenes.Where(n => n.Archivos.Count() > 0).ToList();
        }


        public async Task<bool> guardarResponsable(long idRegimen, string guidRegimen, int idResponsable, string std_responsable = "pendiente")
        {
            ResponsableRegimen rp = new ResponsableRegimen()
            {
                responsableReg_id = 0,
                responsableReg_codigoRegimen = guidRegimen,
                responsableReg_estado = std_responsable,
                Usuariousuario_id = idResponsable,
                Regimenregimen_id = idRegimen
            };

            await __context.ResponsableRegimens.AddAsync(rp);

            await __context.SaveChangesAsync();

            return true;
        }

        //nos traemos los regimenes en relacion al id del responsable.
        public async Task<ICollection<Regimen>> regimenResponsable(int idUsuarioSesion)
        {
            ICollection<Regimen> regimenes = await __context.ResponsableRegimens
                .Include(n => n.regimen)
                    .ThenInclude(n => n.Archivos)
                .Where(n => n.Usuariousuario_id.Equals(idUsuarioSesion))
                .Select(n => n.regimen)
                .ToListAsync();

            ICollection<Archivo> archivos = await __context.Archivos
                .Where(n => n.TipoArchivo.TipoRegId.Equals(1) || n.TipoArchivo.TipoRegId.Equals(5))
                .ToListAsync();

            foreach (var item in regimenes)
            {
                item.Archivos = archivos.Where(n => n.RegimenId.Equals(item.regimen_id)).ToList();
            }

            return regimenes;
        }

        //nos traemos todos los regimenes existentes.. este va a ser usado para nuestro "Usuario Final"
        public async Task<ICollection<Regimen>> regimenResponsable()
        {
            return await __context.ResponsableRegimens
                .Include(n => n.regimen.Archivos)
                    .ThenInclude(n2 => n2.TipoArchivo)
                        .ThenInclude(n3 => n3.TipoReg)
                .Select(r => r.regimen).ToListAsync();
        }

        public async Task<ResponsableRegimen> searchByCodeRegimen(string codigo, int idUsuario)
        {
            var n = await __context.ResponsableRegimens
                .Where(n => n.responsableReg_codigoRegimen.Equals(codigo) && n.Usuariousuario_id.Equals(idUsuario))
                .FirstOrDefaultAsync();

            return n;
        }

    }
}
