using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;

namespace SIS_DIAF.DTO
{
    public class PerfilUsuarioDto
    {
        [Key]
        public long id { set; get; }
        public string rol { set; get; } = null!;
        public string nombre { set; get; } = null!;
        public string cedula { set; get; } = null!;
        public string correo { set; get; } = null!;
        public string estado { set; get; } = null!;
        
        public string mensaje { set; get; } =  "";
        public string tipo { set; get; } =  "Inicio de Sesion";
    }
}
