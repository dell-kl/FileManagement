using SIS_DIAF.DTO;
using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.Models.ModelViews
{
    public class RolesModelViews
    {
        public IEnumerable<Rol> Roles { set; get; }
        
        [Required(ErrorMessage = "Error en el registro xd")]
        public RegistroRolDto RegistroRolDto { set; get; }
    }
}
