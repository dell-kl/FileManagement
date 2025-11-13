using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.DTO
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Ingresa número de cédula")]
        public string usuario { set; get; } = null!;

        [Required(ErrorMessage = "Ingrese una contraseña")]
        [DataType(DataType.Password)]
        public string password { set; get; } = null!;

        public bool AIniciadoSesion { set;  get;  }
    }
}
