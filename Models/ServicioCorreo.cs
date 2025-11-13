using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIS_DIAF.Models
{
    public class ServicioCorreo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string srVCorreo_id { set; get; }
        public string srvCorreo_email { set; get; }
        public string srvCorreo_password { set; get; }
        public string srvCorreo_host { set; get; }
        public int srvCorreo_port { set; get; } 
    }
}
