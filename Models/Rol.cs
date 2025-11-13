using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long rol_id { set; get; }

        public string rol_descripcion { set; get; } = null!;

        public string rol_estado { set; get; } = null!;

        public DateTime rol_fecha_creacion { get; set; } = DateTime.Now;

        public virtual ICollection <Usuario> Usuarios { get; } = new List<Usuario>();
    }
}
