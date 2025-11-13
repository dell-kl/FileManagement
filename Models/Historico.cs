using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.Models
{
    public class Historico
    {
        [Key]
        public long historico_id {  get; set; }
    
        public string ? historico_archivoRuta {  get; set; }
    
        public string ? historico_guidRegimen {  get; set; }

        public string ? historico_guidArchivo {  get; set; }

        public string ? historico_tipoArchivo { get; set; } 
    
        public DateTime ? historico_fecha {  get; set; }
    }
}
