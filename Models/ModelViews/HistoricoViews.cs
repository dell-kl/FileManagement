namespace SIS_DIAF.Models.ModelViews
{
    public class HistoricoViews
    {
        public ICollection<Historico> historico { set; get; } = null!;
        public ICollection<TipoArchivo> tipoArchivo { set; get; } = null!;
        public string Regimen { set; get; } = null!;
        public string Guid { set; get; } = null!;

    }
}
