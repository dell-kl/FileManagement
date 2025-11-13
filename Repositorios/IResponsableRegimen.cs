using SIS_DIAF.Models;

namespace SIS_DIAF.Repositorios
{
    public interface IResponsableRegimen
    {
        Task<bool> guardarResponsable(
            long idRegimen,
            string guidRegimen,
            int idResponsable,
            string std_responsable = "asignado"
        );

    }
}
