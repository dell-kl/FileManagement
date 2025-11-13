namespace SIS_DIAF.Models.ModelViews
{
    public class RegimenCompletadosModelView
    {
        public ICollection<Regimen> regimenes;
        public ICollection<Usuario> usuarios;
        public ICollection<TipoReg> tipoRegs;
    }
}
