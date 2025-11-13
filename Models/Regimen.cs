using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class Regimen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long regimen_id { get; set; }

        public string regimen_guid { get; set; } = null!;


        [Required]
        public string regimen_cod { get; set; } = null!;

        [Required]
        public decimal regimen_presupuesto { set; get; } = 0.0m; 


        [Required]
        public string regimen_objetivo { get; set; } = null!;


        [Required]
        public DateTime? regimen_fecha_creacion { get; set; } = DateTime.Now;

        public virtual ICollection<Archivo> Archivos { get; set;  } = new List<Archivo>();

        // public long UsuarioId { get; set; }

        // public Usuario Usuario { get; set; } = null!;

        public virtual ICollection<ResponsableRegimen> responsableRegimen { get; } = new List<ResponsableRegimen>();

        public string obtenerTipo()
        {
            return this.Archivos.First().TipoArchivo.TipoReg.nombre_tipo_reg;
        }


        public float getProgreso(int tip_reg = 1)
        {
            //tenmos que filtrar pr los distitnos archivos.
            /*var arch = Archivos
                .Where(n => n.TipoArchivo.TipoReg.tiporeg_id.Equals(tip_reg))
                .ToList();*/
            
            int num = Archivos.Where(a => a.archivo_estado.Equals("subido")).ToList().Count;
            if (num == 0) return 0;
            double total = Archivos.Count;
            return (float)Math.Round((num /(double)Archivos.Count) * 100, 2);
        }

    }
}
    