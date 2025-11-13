using SIS_DIAF.DTO;

namespace SIS_DIAF.Models.ModelViews
{
    public class UsuariosModelViews
    {
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public RegistroUsuarioDto registroUsuarioDTO { get; set; } = null!;
        public ICollection<Rol> listaRoles { get; set; } = new List<Rol>();
        public ICollection<Sucursal> listaSucursales { get; set; } = new List<Sucursal>();
    }
}
