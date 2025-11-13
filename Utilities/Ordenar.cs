using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using SIS_DIAF.Models;
using SIS_DIAF.Models.ModelViews;
using System.Text.Json.Nodes;

namespace SIS_DIAF.Utilities
{   
    public static class Ordenar<T> where T: class
    {
        private static int _pag = 5;

        public static List<Archivo> OrdearPrioridadArchivos(List<Archivo> _arch)
        {
            return _arch.OrderBy(n => n.TipoArchivo.tip_prioridad).ToList<Archivo>();

            /*
            var archivos = regimen.Archivos
                .OrderBy(x => x.TipoArchivo.tip_prioridad)
                .ToList<Archivo>();
            
            return archivos;*/
        }

        public static Dictionary<string, object> Atras(int pagina, ICollection<T> lista)
        {
            Dictionary<string, object> datos = new Dictionary<string, object>();

            if (pagina <= 0)
            {
                datos.Add("b-back", "true"); //disabled -> true | false
                datos.Add("b-s-back", "disabled");

                datos.Add("b-next", "false");
                datos.Add("b-s-next", "enabled");

                datos.Add("pagina", pagina);
                datos.Add("info", lista.Take(5).ToList());

                return datos;
            }

            var resultados = lista
                    .Skip(_pag * pagina) // Omitir los primeros 5
                    .Take(5) // Tomar los siguientes 5
                    .ToList();


            datos.Add("b-back", "false"); //disabled -> true | false
            datos.Add("b-s-back", "enabled");

            datos.Add("b-next", "false");
            datos.Add("b-s-next", "enabled");

            datos.Add("pagina", pagina);
            datos.Add("info", resultados);

            return datos;

        }

        public static Dictionary<string, object> Siguiente(int pagina, ICollection<T> lista)
        {
            Dictionary<string, object> datos = new Dictionary<string, object>();
            
            if (pagina == 0)
            {
                datos.Add("pagina", 0);
                datos.Add("info", lista.Take(_pag).ToList());
                
                datos.Add("b-back", "true"); //disabled -> true | false
                datos.Add("b-s-back", "disabled"); 
                
                if ( lista.Count() < _pag)
                {
                    datos.Add("b-next", "true");
                    datos.Add("b-s-next", "disabled");
                }
                else
                {
                    datos.Add("b-next", "false");
                    datos.Add("b-s-next", "enabled");
                }

                return datos;
            }

            int pag = _pag * pagina;
            int cantidad = _pag;
            
            var resultados = lista
                    .Skip(pag) // Omitir los primeros 5
                    .Take(cantidad) // Tomar los siguientes 5
                    .ToList();

            if ( lista.Count() <= (_pag*(pagina+1)) )
            {
                datos.Add("b-next", "true");
                datos.Add("b-s-next", "disabled");

                datos.Add("b-back", "false"); //disabled -> true | false
                datos.Add("b-s-back", "enabled");
            }
            else {
                datos.Add("b-next", "false");
                datos.Add("b-s-next", "enabled");

                datos.Add("b-back", "false");
                datos.Add("b-s-back", "enabled");
            }

            datos.Add("pagina", pagina);
            datos.Add("info", resultados);

            return datos;
        }
    }
}
