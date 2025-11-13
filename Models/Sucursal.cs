using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class Sucursal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long sucursal_id { get; set; }

        [Required]
        public string ? sucursal_nombre { get; set;}

        [Required]
        public DateTime sucursal_fechaCreacion { get; set; }

        public string sucursal_estado { get; set; } = null!;

        // relacion sucursal y 
        public virtual ICollection <Usuario> Usuarios { get;} = new List<Usuario>();

    }
}
