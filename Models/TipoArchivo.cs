using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class TipoArchivo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long tipoarchivo_id { get; set; }

        [Required]
        public string tipo_nombre { get; set; } = null!;

        public string ? tip_descripcion { get; set;}

        public int tip_prioridad { get; set; }

        [Required]
        public DateTime tip_fecha_creacion { get; set; } = DateTime.Now;

        //relacion de archivo y tipo

        public long TipoRegId { get; set; }
        public virtual TipoReg TipoReg { get; set; } = null!;

        public virtual ICollection <Archivo>  Archivos { get; } = new List<Archivo>();
    }
}
