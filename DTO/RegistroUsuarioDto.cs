using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.DTO
{
    public class RegistroUsuarioDto : PassUser
    {

        [Required(ErrorMessage = "Ingresa nombre y apellido del usuario")]
        public string nombre { set; get; } = null!;

        [Required(ErrorMessage = "Ingresa la cédula del usuario")]
        [StringLength(10, ErrorMessage = "La cédula debe contener 10 dígitos")]
        public string cedula { set; get; } = null!;

        [Required(ErrorMessage = "Ingresa un correo de contacto del usuario")]
        [DataType(DataType.EmailAddress)]
        public string email { set; get; } = null!;

        [Required(ErrorMessage = "Ingresa un número de celular o telefónico de contacto")]
        [StringLength(10, ErrorMessage = "el número de contacto debe contener 10 dígitos")]
        public string celular { set; get; } = null!;

        [Required(ErrorMessage = "Ingresa el estado actual del usuario.")]
        [RegularExpression(@"^activo|inactivo$", ErrorMessage = "Especifica el estado correcto del usuario.")]
        public string estado { set; get; } = null!;

        [Required(ErrorMessage = "Seleccione el rol para el usuario.")]
        public long rol {  set; get; } 

        [Required(ErrorMessage = "Seleccione una sucursal para el usuario.")]
        public long sucursal {  set; get; }

    }

    public class PassUser
    {
        public long id { set; get; } = 0;

        [Required(ErrorMessage = "Ingresa una contraseña segura para el usuario")]
        [MinLength(8,ErrorMessage ="La clave debe tener mas de 8 carcteres")]
        [DataType(DataType.Password)]
        public virtual string clave { set; get; } = null!;
    }

    public class EditUsuarioDto : RegistroUsuarioDto
    {
        
        public override string ? clave { set; get; } 
    }
}
