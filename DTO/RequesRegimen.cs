namespace SIS_DIAF.DTO
{
    public record RequesRegimen
    (
        long tipo,
        string objetivo,
        long usuario,
        decimal presupuesto,
        long sucursal
    );
}

