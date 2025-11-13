using SIS_DIAF.Models;

namespace SIS_DIAF.DTO
{
    public class ProcesosEnCursoDto
    {
        public string CodigoRegimen { set; get; } = null!;
        public string GuidRegimen { set; get; } = null!;
        public string Tipo { set; get; } = null!;
        public int tipoRegimen { set; get; }
        public List<Archivo> Archivos { set; get; }
    }
}
