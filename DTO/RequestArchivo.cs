namespace SIS_DIAF.DTO
{
    public record RequestArchivos(
        IFormFileCollection file, 
        long id_archivo,
        string guid_a,
        string guid_r,
        string tipo,
        string tipoNombreArchivo = "ninguno"
    );
}
