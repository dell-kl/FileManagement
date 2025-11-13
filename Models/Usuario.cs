using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long usuario_id { set; get;  }

        public Guid usuario_guid { set; get; }

        [Required]
        public string ? usuario_nombre {  set; get; }

        [Required]
        [StringLength(10)]
        public string ? usuario_cedula {  set; get; }

        [Required]
        public string ? usuario_email { set; get; }

        [Required]
        [StringLength(10)]
        public string? usuario_celular { set; get; }

        [Required]
        public string ? usuario_passwd { set; get; }

        public string usuario_estado { set; get; } = null!;


        //numero de intentos que tiene el usuario para poder loguear... despues de eso se vuelve inactivo el perfil.
        [DefaultValue(3)]
        public int usuario_nIntentos { set; get; } = 3;

        public long RolId {  set; get; }
        public virtual Rol Rol { set; get; } = null!;

        public long SucursalId { set; get; }
        public Sucursal Sucursal { set; get; } = null!;

        //Relacion de uno a muchos con regimen
        //public virtual ICollection<Regimen> Regimens { get;} = new List<Regimen>();
        public virtual ICollection<ResponsableRegimen> responsableRegimen { get; set; } = new List<ResponsableRegimen>();

    }
}
