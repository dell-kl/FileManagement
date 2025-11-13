using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class Archivo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long archivo_id { get; set; }

        public string archivo_guid { get; set; } = null!;
            
        public string ? archivo_nombre { get; set; } 
   
        public string ? archivo_ruta {  get; set; }

        public DateTime ? fecha_subida { get; set; } 

        [Required]
        public string archivo_estado { get; set; } = "Sin archivo";

        public long RegimenId { get; set; }
        public virtual Regimen Regimen { get; set; } = null!;

        public long TipoArchivoId { get; set; }
        public TipoArchivo TipoArchivo { get; set; } = null!;


        /*Este campo lo vamos a utilizar para mostrar 
        el titulo de cada archivo en la parte de administrador logistica. 

        No se incluira jamas este campo en la tabla Archivo
        */
        [NotMapped]
        public string? tipoNombre { set; get; } = "ninguno";
    }
}
