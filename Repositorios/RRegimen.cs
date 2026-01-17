using Microsoft.EntityFrameworkCore;
using SIS_DIAF.Models;
using SIS_DIAF.DTO;
using System.Data;
using System.Collections;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.ObjectModel;

namespace SIS_DIAF.Repositorios
{
    public class RRegimen : IRepositorio<Regimen>
    {
        private SistemaDiafContext _contex;
        public RRegimen(SistemaDiafContext contex)
        {
            _contex = contex;
        }

        public async Task<bool> Create(Regimen x)
        {
            if (x == null)
            {
                return false;
            }
            _contex.Regimens.Add(x);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<bool> Delete(long id)
        {
            var rem = await GetxId(id);
            if (rem == null) { 
                return false;
            }
            _contex.Regimens.Remove(rem);
            return await _contex.SaveChangesAsync() > 0;
        }

        public async Task<ICollection<Regimen>> Listar()
        {
            ICollection<Regimen> regimenes = await _contex.Regimens
                .Include(n => n.Archivos)
                    .ThenInclude(n2 => n2.TipoArchivo)
                        .ThenInclude(n3 => n3.TipoReg)
                .Include(n2 => n2.responsableRegimen)
                    .ThenInclude(n3 => n3.usuario)
                        .ThenInclude(n4 => n4.Rol)
                .Select(n => n).ToListAsync();

            ICollection<Archivo> archivos = await _contex.Archivos
                .Where(n => n.TipoArchivo.TipoRegId.Equals(1) || n.TipoArchivo.TipoRegId.Equals(5))
                .ToListAsync();

            foreach (var item in regimenes)
            {
                item.Archivos = archivos.Where(n => n.RegimenId.Equals(item.regimen_id)).ToList();
            }

            return regimenes;
        }

        public async Task<ICollection<ResponsableRegimen>> Listar(string tipo)
        {
            ICollection<ResponsableRegimen> regimenes =  await _contex.ResponsableRegimens
                .Include(n => n.usuario)
                .Include(n => n.regimen)
                    .ThenInclude(n2 => n2.Archivos)
                .Where(n => n.usuario.RolId.Equals(6))
                .ToListAsync();
            
            //En el TipoRegimen debemos decirle que sea distinto de ->1<- GENERAL y ->5<- LOGISTICA
            ICollection<Archivo> archivos = await _contex.Archivos
                .Include(n => n.TipoArchivo)
                    .ThenInclude(n2 => n2.TipoReg)
                .Where(n => !n.TipoArchivo.TipoReg.tiporeg_id.Equals(1) &&
                !n.TipoArchivo.TipoReg.tiporeg_id.Equals(5)
                ).ToListAsync();

            foreach(var item in regimenes)
            {
                item.regimen.Archivos = archivos.Where(n => n.RegimenId.Equals(item.regimen.regimen_id)).ToList();
            }

            return regimenes
                .Where(n => n.regimen.Archivos.Count() > 0).ToList();
        }

        public async Task<ICollection<Regimen>> Listar(int usuarioid)
        {
            var data = await (from rgm in _contex.Regimens 
                              join resp in _contex.ResponsableRegimens on rgm.regimen_id equals resp.Regimenregimen_id
                              where resp.Usuariousuario_id == usuarioid
                              where resp.responsableReg_estado == "asignado"
                              select new Regimen{
                                regimen_id = rgm.regimen_id,
                                regimen_guid = rgm.regimen_guid,
                                regimen_cod = rgm.regimen_cod,
                                regimen_objetivo = rgm.regimen_objetivo,
                                Archivos = new List<Archivo>() 
                              }).ToListAsync();
            
            var archivos = (from dt in data
                         join arc in _contex.Archivos on dt.regimen_id equals arc.RegimenId
                         join tpArc in _contex.TipoArchivos on arc.TipoArchivoId equals tpArc.tipoarchivo_id
                         join tpregs in _contex.TipoRegs on tpArc.TipoRegId equals tpregs.tiporeg_id
                         where tpArc.TipoRegId != 1 && tpArc.TipoRegId != 5
                         select new Archivo{
                             archivo_id = arc.archivo_id,
                             archivo_guid = arc.archivo_guid,
                             archivo_nombre = arc.archivo_nombre,
                             archivo_ruta = arc.archivo_ruta,
                             fecha_subida = arc.fecha_subida,
                             archivo_estado = arc.archivo_estado,
                             RegimenId = arc.RegimenId,
                             Regimen = arc.Regimen,
                             TipoArchivoId = arc.TipoArchivoId,
                             TipoArchivo = arc.TipoArchivo
                         }).ToList();

           
            data.ForEach(d => {
               d.Archivos = archivos.Where(n => n.RegimenId.Equals(d.regimen_id)).ToList();
            });

            return data;
        }

        public async Task<bool> Update(Regimen x)
        {
            if (x == null)
            {
                return false;
            }
            _contex.Regimens.Update(x);
            return await _contex.SaveChangesAsync() > 0;
        }


        public async Task<ICollection<Regimen>> ListXDescripcionLogistic(string entrada, string tipo, long idUsuario)
        {
            ICollection<Regimen> listadoRegimens = await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                .Include(r => r.regimen)
                .Where(u =>
                    u.usuario.Rol.rol_descripcion.ToLower().Equals("logística")
                    &&
                    u.usuario.usuario_id.Equals(idUsuario)
                )
                .Select(e=>e.regimen)
                .ToListAsync();

            foreach (var regimen in listadoRegimens)
            {
                regimen.Archivos = await _contex.Archivos
                 .Include(n => n.TipoArchivo)
                     .ThenInclude(e => e.TipoReg)
                 .Where(n =>
                 (!n.TipoArchivo.TipoReg.tiporeg_id.Equals(1) && !n.TipoArchivo.TipoReg.tiporeg_id.Equals(5)) &&
                 n.RegimenId.Equals(regimen.regimen_id))
                 .ToListAsync();

                if ( tipo.Equals("tipo"))                
                    regimen.Archivos = regimen.Archivos.Where(n => n.TipoArchivo.TipoReg.nombre_tipo_reg.ToLower().Contains(entrada.ToLower())).ToList();

            }

            if ( tipo.Equals("objetivo") )
                listadoRegimens = listadoRegimens.Where(n => n.regimen_objetivo.Contains(entrada)).ToList();

            if (tipo.Equals("tipo"))
                listadoRegimens = listadoRegimens.Where(n => n.Archivos.Count() > 0).ToList();

            return listadoRegimens;
        }

        //este de aqui va a ser usado mas por el perfil del responsable.
        public async Task<ICollection<Regimen>> ListXPalabraClave(string entrada, string tipo, long idUsuario)
        {
            ICollection<Regimen> listadoRegimens = new List<Regimen>();

            
            if ( tipo.Equals("objetivo") )
            {
                listadoRegimens = await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                .Include(r => r.regimen)
                .Where(u =>
                    u.regimen.regimen_objetivo.ToLower().Contains(entrada.ToLower())
                    &&
                    u.usuario.Rol.rol_descripcion.ToLower().Equals("responsable")
                    &&
                    u.usuario.usuario_id.Equals(idUsuario)
                )
                .Select(e => e.regimen)
                .ToListAsync();


                if (listadoRegimens.Any() )
                {
                    foreach (var rR in listadoRegimens)
                    {
                        rR.Archivos = await _contex.Archivos
                            .Include(a => a.TipoArchivo)
                                .ThenInclude(e=>e.TipoReg)
                            .Where(n => 
                                (n.TipoArchivo.TipoRegId.Equals(1) || n.TipoArchivo.TipoRegId.Equals(5))
                                && n.RegimenId.Equals(rR.regimen_id)    
                            )
                            .ToListAsync();
                    }

                }

            }

            return listadoRegimens;
        }

        public async Task<ICollection<ResponsableRegimen>> ListXPalabraClave(string entrada, string tipo, string perfil)
        {
            ICollection<ResponsableRegimen> responsableRegimen = new List<ResponsableRegimen>();

            if ( tipo.Equals("objetivo") )
            {
                //tenemos que hacer un filtrado..
                responsableRegimen = await _contex.ResponsableRegimens
                    .Include(n => n.usuario)
                    .Include(n => n.regimen)
                    .Where(n => 
                        n.usuario.RolId.Equals(6) 
                        && 
                        n.regimen.regimen_objetivo.ToLower().Contains(entrada.ToLower())) //filtrar para que tenga un numero de cedula.
                    .ToListAsync();
            }
            else if ( tipo.Equals("responsable") )
            {
                //tenemos que hacer un filtrado..
                responsableRegimen = await _contex.ResponsableRegimens
                    .Include(n => n.usuario)
                    .Include(n => n.regimen)
                    .Where(a => 
                        a.usuario.RolId.Equals(6) 
                        && 
                        a.usuario.usuario_nombre!.ToLower().Contains(entrada.ToLower())
                    )
                    .ToListAsync();
            }

            //filtrar 

            if (responsableRegimen.Any())
            {
                foreach (var rR in responsableRegimen)
                {
                    rR.regimen.Archivos = (
                        await _contex.Archivos
                            .Include(n => n.TipoArchivo)
                                .ThenInclude(n2 => n2.TipoReg)
                            .Where(n => !n.TipoArchivo.TipoReg.tiporeg_id.Equals(1) &&
                            !n.TipoArchivo.TipoReg.tiporeg_id.Equals(5) && n.RegimenId.Equals(rR.regimen.regimen_id))
                            .ToListAsync()
                    );
                }

            }

            return responsableRegimen;
        }

        public async Task<ICollection<Regimen>> ListXPalabraOPresupuesto(string entrada, string tipo, int presupuesto, int idUsuario = 0)
        {
            ICollection<Regimen> regimenes = (await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.regimen)
                    .ThenInclude(a => a.Archivos)
                        .ThenInclude(t => t.TipoArchivo)
                            .ThenInclude(tp => tp.TipoReg)
                .Where(r => 
                    r.usuario.Rol.rol_descripcion.Equals("responsable")
                )
                .ToListAsync())
                .Select(r => r.regimen)
                .Where(r =>
                    (!idUsuario.Equals(0))
                    ? r.responsableRegimen.First().usuario.usuario_id.Equals(idUsuario)
                    : true
                )
                .Where(r =>
                    (tipo.Equals("objetivo"))
                    ? r.regimen_objetivo.ToLower().Contains(entrada.ToLower())
                    : r.responsableRegimen.First().usuario.usuario_nombre!.Contains(entrada)
                    &&
                    (presupuesto == 3)
                    ? r.regimen_presupuesto >= 500001
                    : (presupuesto == 2)
                    ? r.regimen_presupuesto >= 100001 && r.regimen_presupuesto <= 500000
                    : r.regimen_presupuesto >= 1 && r.regimen_presupuesto <= 100000
                )
                .ToList();


            foreach (var item in regimenes)
            {
                item.Archivos = item.Archivos.Where(
                        t => t.TipoArchivo.TipoRegId.Equals(1) || t.TipoArchivo.TipoRegId.Equals(5)
                ).ToList();
            }

            return regimenes;
        }

        public async Task<ICollection<Regimen>> ListXPalabraClave(string entrada, string tipo)
        {
            ICollection<Regimen> listadoRegimens = new List<Regimen>();

            if ( tipo.Equals("objetivo") )
            {
                listadoRegimens = await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                .Include(r => r.regimen)
                    .ThenInclude(a => a.Archivos)
                .Where(u => u.regimen.regimen_objetivo.ToLower().Contains(entrada.ToLower())
                && u.usuario.Rol.rol_descripcion.ToLower().Equals("responsable"))
                .Select(rg => rg.regimen)
                .ToListAsync();
            }
            else if ( tipo.Equals("responsable") )
            {
                listadoRegimens = await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                .Include(r => r.regimen)
                    .ThenInclude(a => a.Archivos)
                .Where(u => u.usuario.usuario_nombre!.Contains(entrada) 
                    && u.usuario.Rol.rol_descripcion.ToLower().Equals("responsable"))
                .Select(rg => rg.regimen)
                .ToListAsync();
            }

            //sacar la informacion del responsable para estos regimenes. 
            foreach (Regimen regimen in listadoRegimens)
            {
                regimen.responsableRegimen.Add(await _contex.ResponsableRegimens
                    .Include(u => u.usuario)
                        .ThenInclude(r => r.Rol)
                    .Where(u => u.responsableReg_codigoRegimen.Equals(regimen.regimen_guid) &&
                    u.usuario.Rol.rol_descripcion.ToLower().Equals("responsable")).FirstAsync());


                regimen.Archivos = await _contex.Archivos
                    .Include(a=>a.TipoArchivo)
                        .ThenInclude(e=>e.TipoReg)
                    .Where(n =>
                       ( n.TipoArchivo.TipoRegId.Equals(1) || n.TipoArchivo.TipoRegId.Equals(5) )
                        && n.RegimenId.Equals(regimen.regimen_id)
                    )
                    .ToListAsync();
            }


            return listadoRegimens;
        }

        public async Task<ICollection<ResponsableRegimen>> ListxCedula(string cedula, string perfil)
        {
            ICollection<ResponsableRegimen> responableRegimen = new List<ResponsableRegimen>();

            //tenemos que hacer un filtrado..
            responableRegimen = await _contex.ResponsableRegimens
                .Include(n => n.usuario)
                .Include(n => n.regimen)
                .Where(n => n.usuario.RolId.Equals(6) && n.usuario.usuario_cedula!.Equals(cedula)) //filtrar para que tenga un numero de cedula.
                .ToListAsync();

            if ( responableRegimen.Any() )
            {
                foreach(var rR in responableRegimen)
                {
                    rR.regimen.Archivos = (
                        await _contex.Archivos
                            .Include(n => n.TipoArchivo)
                                .ThenInclude(n2 => n2.TipoReg)
                            .Where(n => !n.TipoArchivo.TipoReg.tiporeg_id.Equals(1) && 
                            !n.TipoArchivo.TipoReg.tiporeg_id.Equals(5) && n.RegimenId.Equals(rR.regimen.regimen_id))
                            .ToListAsync()
                    );
                }
            }

            return responableRegimen;
        }

        public async Task<ICollection<Regimen>> ListxPresupuesto(int presupuesto, int idUsuario = 0)
        {
            ICollection<Regimen>  listadoRegimenes = await _contex.Regimens
                    .Include(n => n.Archivos)
                        .ThenInclude(n => n.TipoArchivo)
                            .ThenInclude(n => n.TipoReg)
                    .Include(n => n.responsableRegimen)
                        .ThenInclude(u => u.usuario)
                            .ThenInclude(r => r.Rol)
                    .Where(r => 
                        (!idUsuario.Equals(0))
                        ? r.responsableRegimen.Where(n => n.usuario.usuario_id.Equals(idUsuario)).Select(n => n.usuario).First().usuario_id.Equals(idUsuario)
                        : true
                    )
                    .Where(r =>
                        (presupuesto == 3)
                        ? r.regimen_presupuesto >= 500001
                        : (presupuesto == 2)
                        ? r.regimen_presupuesto >= 100001 && r.regimen_presupuesto <= 500000
                        : r.regimen_presupuesto >= 1 && r.regimen_presupuesto <= 100000
                    )
                    .ToListAsync();

            foreach (var item in listadoRegimenes)
            {
                item.Archivos = item.Archivos.Where(
                        t => t.TipoArchivo.TipoRegId.Equals(1) || t.TipoArchivo.TipoRegId.Equals(5)
                ).ToList();
            }

            return listadoRegimenes;
        }

        public async Task<ICollection<Regimen>> ListXCedula_Presupuesto(string cedula, int presupuesto )
        {
            ICollection<Regimen> regimenes = ( await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                    .ThenInclude(r => r.Rol)
                .Include(r => r.regimen)
                    .ThenInclude(a => a.Archivos)
                        .ThenInclude(t => t.TipoArchivo)
                            .ThenInclude(tp => tp.TipoReg)
                .Where(u => u.usuario.usuario_cedula!.Equals(cedula))
                .Where(r => 
                    (presupuesto == 3) 
                    ?  r.regimen.regimen_presupuesto >= 500001
                    : (presupuesto == 2)
                    ? r.regimen.regimen_presupuesto >= 100001 && r.regimen.regimen_presupuesto <= 500000
                    : r.regimen.regimen_presupuesto >= 1 && r.regimen.regimen_presupuesto <= 100000
                )
                .ToListAsync() )
                .Select(r => r.regimen)
                .ToList(); 

            foreach (var item in regimenes)
            {
                item.Archivos = item.Archivos.Where(
                        t => t.TipoArchivo.TipoRegId.Equals(1) || t.TipoArchivo.TipoRegId.Equals(5)
                ).ToList();
            }


            return regimenes;
        }

        public async Task<ICollection<Regimen>> ListxCedula(string cedula)
        {
            ICollection<Regimen> listadoRegimenes = await _contex.ResponsableRegimens
                .Include(u => u.usuario)
                .Include(r => r.regimen)
                .Where(u => u.usuario.usuario_cedula!.Equals(cedula))
                .Select(rg => rg.regimen)
                .ToListAsync();

            if ( listadoRegimenes.Any() )
            {
                ResponsableRegimen respRegimen = new ResponsableRegimen()
                {
                    usuario = (await _contex.ResponsableRegimens
                    .Include(n => n.usuario)
                        .ThenInclude(o => o.Rol)
                    .Where(r => r.usuario.usuario_cedula.Equals(cedula))
                    .Select(u => u.usuario).FirstAsync()
                    )
                };
            
                foreach(Regimen regimen in listadoRegimenes)
                {
                    regimen.responsableRegimen.Add(respRegimen);

                    regimen.Archivos = await _contex.Archivos
                        .Include(n => n.TipoArchivo)
                            .ThenInclude(e => e.TipoReg)
                    .Where(n => ( n.TipoArchivo.TipoRegId.Equals(1) || n.TipoArchivo.TipoRegId.Equals(5) )
                    && n.RegimenId.Equals(regimen.regimen_id))
                    .ToListAsync();
                }
            }

            
            return listadoRegimenes;
        }


        public async Task<long> getIdCurrent(string code) {

            var ultimo = await _contex.Regimens.Where(r => r.regimen_guid.Equals(code)).FirstOrDefaultAsync();
            return ultimo!.regimen_id;
        }

        public async Task<Regimen> GetxId(long id)
        {
            var rem = await _contex.Regimens.FindAsync(id);
            return rem!;
        }

        public async Task<Regimen> GetRegimenArchivosLogistica(string guid, int tip_reg)
        {
            //seleccionamos por el guid correspondiente y por el id del usuario del perfil que inicia sesion...
            var registro = await _contex.Regimens
                .Include(t => t.Archivos)
                    .ThenInclude(o => o.TipoArchivo)
                        .ThenInclude(p => p.TipoReg)
                .Where(r =>
                    r.regimen_guid.Equals(guid)
                    )
                .FirstAsync();

            ICollection<Archivo> archivos =  await _contex.Archivos
                .Where(n => n.TipoArchivo.TipoReg.tiporeg_id.Equals(tip_reg) && n.Regimen.regimen_id.Equals(registro.regimen_id))
                .ToListAsync();

            registro.Archivos = archivos;

            return registro!;
        }

        public async Task<List<Archivo>> GetArchivos(string guid, int tip_reg)
        {
            var registro2 = await _contex.Archivos
                .Include(r => r.Regimen)
                .Include(a => a.TipoArchivo)
                    .ThenInclude(a2 => a2.TipoReg)
                .Where(n => n.Regimen.regimen_guid.Equals(guid)
                && n.TipoArchivo.TipoReg.tiporeg_id.Equals(tip_reg))
                .ToListAsync();

            return registro2;
        }

        public async Task<Regimen> GetRegimenArchivos(string guid)
        {
            return await _contex.Regimens
                .Include(a => a.Archivos)
                    .ThenInclude(t => t.TipoArchivo)
                        .ThenInclude(tp => tp.TipoReg)
                .Where(n => n.regimen_guid.Equals(guid)).FirstAsync();
        }

        public async Task<ICollection<Archivo>> GetArchivosLogistica(string guid)
        {
            var _regimen = await _contex.Regimens
                .Include(n => n.responsableRegimen)
                    .ThenInclude(n2 => n2.usuario)
                        .ThenInclude(n3 => n3.Rol)
                .Where(n => n.regimen_guid.Equals(guid))
                .FirstOrDefaultAsync();

            ICollection<Archivo> _archivos = new List<Archivo>();
            
            if (_regimen != null)
            {
                _archivos = await _contex.Archivos
                    .Include(n => n.TipoArchivo)
                        .ThenInclude(n2 => n2.TipoReg)
                    .Where(n => n.RegimenId.Equals(_regimen.regimen_id) &&
                    !n.TipoArchivo.TipoRegId.Equals(1) && !n.TipoArchivo.TipoRegId.Equals(5))
                    .ToListAsync();
            }

            return _archivos;
        }

        public async Task<List<Archivo>> GetArchivosFinanzas(string guid, int tip_reg)
        {
            var registro2 = await _contex.Archivos
                .Include(r => r.Regimen)
                .Include(a => a.TipoArchivo)
                    .ThenInclude(a2 => a2.TipoReg)
                .Where(n => n.Regimen.regimen_guid.Equals(guid)
                && n.TipoArchivo.tipo_nombre.Equals("LIQUIDACIÓN")
                && n.TipoArchivo.TipoReg.tiporeg_id.Equals(tip_reg))
                .ToListAsync();

            return registro2;
        }

        public async Task<List<Archivo>> GetArchivosJuridico(string guid, int tip_reg)
        {
            var registro2 = await _contex.Archivos
                .Include(r => r.Regimen)
                .Include(a => a.TipoArchivo)
                    .ThenInclude(a2 => a2.TipoReg)
                .Where(n => n.Regimen.regimen_guid.Equals(guid)
                && n.TipoArchivo.tipo_nombre.Equals("CONTRATO FAE-DIAF")
                && n.TipoArchivo.TipoReg.tiporeg_id.Equals(tip_reg))
                .ToListAsync();

            return registro2;
        }

        public async Task<List<string>> obtenerInformacionSubArchivosRegimen(string guid)
        {
            var _regimen = await _contex.Regimens
                .Include(n => n.responsableRegimen)
                    .ThenInclude(n2 => n2.usuario)
                        .ThenInclude(n3 => n3.Rol)
                .Where(n => n.regimen_guid.Equals(guid))
                .FirstAsync();

            ICollection<Archivo> _archivos = await _contex.Archivos
                .Include(n => n.TipoArchivo)
                    .ThenInclude(n2 => n2.TipoReg)
                .Where(n => n.RegimenId.Equals(_regimen.regimen_id) && 
                !n.TipoArchivo.TipoRegId.Equals(1) && !n.TipoArchivo.TipoRegId.Equals(5))
                .ToListAsync();

            if (  !_archivos.Any() )
            {
                return new List<string>();
            }

            ICollection<ResponsableRegimen> responsable = _archivos.First().Regimen.responsableRegimen;
            Usuario responsableUsuario = responsable.Where(n => n.usuario.Rol.rol_descripcion.Equals("logística")).Select(n => n.usuario).First();

            Regimen n = new Regimen();
             n.Archivos = _archivos;
            return new List<string>() {
                _archivos.First().TipoArchivo.TipoReg.nombre_tipo_reg,
                _archivos.First().TipoArchivo.TipoReg.tiporeg_id.ToString(),
                n.getProgreso().ToString(),
                responsableUsuario.usuario_nombre!
            };
        }

        public async Task<Regimen> GetxCod(string c)
        {
            var rem = await _contex.Regimens.Where<Regimen>(r => r.regimen_cod.Equals(c))
                .FirstOrDefaultAsync();

            return rem!;
        }


        //este metodo se encarga de verificar cuales son los regimenes asignados
        //al de logistica 'normal'
        public async Task<ICollection<ResponsableRegimen>> verificarRegimenParaLogistica(int idUsuarioLogistica)
        {
            ICollection<ResponsableRegimen> regimenLogistica = await _contex.ResponsableRegimens
                .Where(resp => resp.Usuariousuario_id.Equals(idUsuarioLogistica) && resp.responsableReg_estado.Equals("pendiente") )
                .Include(t => t.usuario)
                .Include(e => e.regimen)
                .ToListAsync();

            return regimenLogistica;
        }


        public async void filtrarArchivosXTipoLogistica(
            ICollection<Regimen> datoRegimen = null, 
            ICollection<ResponsableRegimen> datoResponsableRegimen = null)
        {

            //este codigo solo trae todos los archivos, que tenga el TipoRegimen distinto de GENERAL
            //y LOGISTICA...
            object datos = datoRegimen.Any() ? datoRegimen : null;

            if (datos == null)
                datos = datoResponsableRegimen.Any() ? datoResponsableRegimen : null;

            
        }


        public async Task<Regimen> obtenerUltimoRegimen()
        {
            var dato = await _contex.Regimens
                .Include(r => r.responsableRegimen)
                    .ThenInclude(u => u.usuario)
                        .ThenInclude(r => r.Rol)
                .OrderBy(r => r.regimen_id)
                .ToListAsync();

            
            return dato.Last(); 
        }

    }
}
