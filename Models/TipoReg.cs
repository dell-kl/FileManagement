using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class TipoReg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long tiporeg_id { get; set; }

        [Required]
        public string nombre_tipo_reg { get; set; } = null!;

        [Required]
        public DateTime tipo_fecha_creacion { get; set; } = DateTime.Now;

        public virtual ICollection<TipoArchivo> TipoArchivos { get; } = new List<TipoArchivo>();
    }
}