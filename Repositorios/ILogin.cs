using SIS_DIAF.DTO;

namespace SIS_DIAF.Repositorios
{
    public interface ILogin
    {
        Task<PerfilUsuarioDto> Filtrar(LoginDto t);
    }
}
