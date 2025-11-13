using SIS_DIAF.Models;

namespace SIS_DIAF.Utilities
{
    public class HtmlPdfGenerator
    {
        public string datos { set; get; } = "";
        public string codigoRegimen { set; get; } = "";

        public string objetivoRegimen { set; get; } = "";


        public string generHtml()
        {

            string html = @"
                <style>
                    table {
                        width: 100%;
                        border-collapse: collapse;
                        font-family: Arial, sans-serif;
                    }

                    th, td {
                        border: 1px solid #dddddd;
                        text-align: left;
                        padding: 8px;
                    }

                    th {
                        background-color: #f2f2f2;
                        font-weight: bold;
                    }

                    tr:nth-child(even) {
                        background-color: #f9f9f9;
                    }

                    .table-header {
                        text-align: center;
                        font-size: 1.2em;
                        padding: 10px;
                    }

                    #correctoMensaje {
                        padding-right:5px;
                        color:#157347;
                        font-weight:bold;
                        padding:4px;
                        font-size:20px;
                    }   

                    #errorMensaje {
                        padding-right:5px;
                        color:#BB2D3B;
                        font-weight:bold;
                        padding:4px;
                        font-size:20px;
                    }
                </style>
                <h2>Control de Documentos de Régimen Presentado</h2>
                <table border=""1"" style=""width:100%; border-collapse: collapse;"">
                    <thead>
                        <tr>
                            <th colspan=""4"">
                                Dirección de Industria Aeronáutica <br />
                                DIAF - Gerencia Comercial <br />
                                Control de Documentos de Régimen Presentado
                            </th>
                        </tr>
                        <tr>
                            <th colspan=""4"">Nro Régimen: " + codigoRegimen+ @"</th>
                        </tr>
                        <tr>
                            <th colspan=""4"">Objetivo Régimen: "+objetivoRegimen+@"</th>
                        </tr>
                        <tr>
                            <th>Nombre</th>
                            <th>Detalle</th>
                            <th>Observación</th>
                        </tr>
                    </thead>
                    <tbody>
                        " + datos+@"
                    </tbody>
                </table>
            ";

            return html;
        }

        public void agregarDatosPdf(Regimen regimen)
        {
            string tr = "";

            foreach(Archivo arch in regimen.Archivos)
            {
                string nombreTipoRegimen = arch.TipoArchivo.TipoReg.nombre_tipo_reg;

                if ( nombreTipoRegimen.Equals("GENERAL") )
                {
                    tr += @$"
                    <tr>
                        <td>{arch.TipoArchivo.tipo_nombre}</td>
                        <td>
                            {(arch.archivo_estado.Equals("subido") ? "<span id='correctoMensaje'>Cumple ✔️</span>" : "<span id='errorMensaje'>No Cumple ❌</span>")}
                        </td>
                        <td>
                            {(arch.archivo_estado.Equals("subido") ? "El archivo del proceso ha sido subido exitosamente" : "Falta por subir el archivo de este proceso")}
                        </td>
                    </tr>
                    ";

                    if ( arch.TipoArchivo.tipo_nombre.Equals("REQUERIMIENTO") )
                    {
                        //analizar si se subio los tres archivos del supervisor de logistica
                        int ArchivosSubidosSupervisorLogistc = regimen.Archivos.Where(n => n.TipoArchivo.TipoReg.nombre_tipo_reg.Equals("LOGISTICA") && n.archivo_estado.Equals("subido")).Count();

                        tr += @$"
                            <tr>
                                <td>LOGISTICA</td>
                                <td>
                                    {(ArchivosSubidosSupervisorLogistc.Equals(3) ? "<span id='correctoMensaje'>Cumple ✔️</span>" : "<span id='errorMensaje'>No Cumple ❌</span>")}
                                </td>
                                <td>
                                    {(ArchivosSubidosSupervisorLogistc.Equals(3) ? "El archivo del proceso ha sido subido exitosamente" : "Falta por subir algunos archivos de este proceso")}
                                </td>
                            </tr>
                        ";
                    }
                }
            }

            datos = tr;
            codigoRegimen = regimen.regimen_cod;
            objetivoRegimen = regimen.regimen_objetivo;
        }
    }
}
