using SIS_DIAF.Models;

namespace SIS_DIAF.DTO.DTOVEIW
{
    public record DtoViewRegimen
    (
        string codigo,
        string guid,
        List<Archivo> archivos
        );
}
