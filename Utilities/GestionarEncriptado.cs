using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.Security;

namespace SIS_DIAF.Utilities
{
    public class GestionarEncriptado
    {
        /*
        
        Esta clase de aqui
        vamos a utilizar mucho para encriptar y desencriptar ciertos datos del cual 
        repetimos mucho en nuestros controladores, principalmente para la parte de la
        funcionalidad del paginador.... 

         */
        private readonly EncriptacionPass _security;

        public GestionarEncriptado(EncriptacionPass security)
        {
            _security = security;
        }

        public Dictionary<string, string> encriptarDatos(Dictionary<string,string> datos)
        {
            datos["tipo"] = _security.EncriptrarUrl("0");
            datos["pagina"] = _security.EncriptrarUrl("0");
            datos["direccion"] = _security.EncriptrarUrl("siguiente");
            datos["entrada"] = _security.EncriptrarUrl("vacio");
            return datos;
        }

        private Dictionary<string,string> verificarEncriptadoDatos(Dictionary<string,string> datos)
        {
            datos["tipo"] = (string.IsNullOrEmpty(datos["tipo"])) ? _security.EncriptrarUrl("0") : datos["tipo"];
            datos["entrada"] = (string.IsNullOrEmpty(datos["entrada"])) ? _security.EncriptrarUrl("vacio") : datos["entrada"];

            if ( datos.ContainsKey("presupuesto") )
                datos["presupuesto"] = (string.IsNullOrEmpty(datos["presupuesto"])) ? _security.EncriptrarUrl("0") : datos["presupuesto"];

            datos["pagina"] = (string.IsNullOrEmpty(datos["pagina"])) ? _security.EncriptrarUrl("0") : datos["pagina"];
            datos["direccion"] = (string.IsNullOrEmpty(datos["direccion"])) ? _security.EncriptrarUrl("siguiente") : datos["direccion"];

            return datos;
        }

        public Dictionary<string, string> SeguridadEncriptado(Dictionary<string,string> datos)
        {
            //primero verificamos por los datos para encriptar.
            datos = verificarEncriptadoDatos(datos);
            datos = VerificarDesencriptacionDatos(datos);
            return datos;
        }


        private Dictionary<string,string> VerificarDesencriptacionDatos(Dictionary<string,string> datos)
        {
            try
            {
                datos["tipo"] = _security.DesencriptarUrl(datos["tipo"]); // -> de tipo numero
                datos["pagina"] = _security.DesencriptarUrl(datos["pagina"]);
                datos["direccion"] = _security.DesencriptarUrl(datos["direccion"]);
                datos["entrada"] = _security.DesencriptarUrl(datos["entrada"]); // -> de tipo numero

                if ( datos.ContainsKey("presupuesto") )
                    datos["presupuesto"] = _security.DesencriptarUrl(datos["presupuesto"]);

                return datos;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
