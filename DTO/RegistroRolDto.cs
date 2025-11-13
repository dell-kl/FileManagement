using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace SIS_DIAF.DTO
{
    public class RegistroRolDto
    {
        public long id { set; get; } = 0;

        [Required(ErrorMessage = "Ingresa un nombre que identifique a tu rol nuevo.")]
        [RegularExpression(@"^([A-Za-z]+\s*)+$", ErrorMessage = "El rol no debe contener números u carácteres especiales.")]
        public string descripcion { set; get; } = null!;

        [Required(ErrorMessage = "Ingresa un estado a tu nuevo rol")]
        [RegularExpression(@"^activo|inactivo$", ErrorMessage = "Especifica el estado correcto del usuario.")]
        public string estado { set; get; } = null!;

        public bool estadoRegistro { set; get; } = false;
    }
}
