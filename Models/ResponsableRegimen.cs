using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.Models
{
    public class ResponsableRegimen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int responsableReg_id { set; get; }

        [Required(ErrorMessage = "Debes ingresar un codigo de regimen")]
        public string responsableReg_codigoRegimen { set; get; }

        // este de aqui nos permitira verificar por estados si se asigno la responsabilidad.
        public string responsableReg_estado { set; get; } = null;

        [Column("responsableReg_usuarioId")]
        public long Usuariousuario_id { set; get; }
        public virtual Usuario usuario { set; get; } = null!;

        [Column("responsableReg_regimenId")]
        public long? Regimenregimen_id { set; get; }
        public virtual Regimen regimen { set; get; } = null!;
    }
}
