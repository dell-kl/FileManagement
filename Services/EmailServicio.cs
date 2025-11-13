using MimeKit;
using MailKit.Net.Smtp;
using System.Net;
using SIS_DIAF.Repositorios;
using SIS_DIAF.Models;
using SIS_DIAF.Security;
using Microsoft.IdentityModel.Tokens;
using SIS_DIAF.DTO;



namespace SIS_DIAF.Services
{
    public class EmailServicio : IEmailService
    {
        private readonly ICorreoRepository _correoRepository;
        private ServicioCorreo _servicioCorreo { set; get; } = new ServicioCorreo();
        private EncriptacionPass _encriptacion;
        private readonly RRegimen _regimen;
        private readonly RUsuario _usuario;

        /* Propiedades para la configuracion de la direccion de correo electronico. */
        private string _smtp;
        private string _from;
        private string _pass;
        private int _port;

        public EmailServicio(
            ICorreoRepository correoRepository,
            EncriptacionPass encriptacionPass,
            RRegimen regimen,
            RUsuario usuario
        )
        {
            _correoRepository = correoRepository;
            _encriptacion = encriptacionPass;
            _regimen = regimen;
            _usuario = usuario;
        }

        public async void obtenerDatosDesencriptadosCorreo()
        {
            _servicioCorreo = await _correoRepository.obtenerDatosCorreo();

            //vamos a desencriptar los respectivos campos para poder usarlos...
            _from = _encriptacion.DesencriptarDatos(_servicioCorreo.srvCorreo_email);
            _smtp = _encriptacion.DesencriptarDatos(_servicioCorreo.srvCorreo_host);
            _pass = _encriptacion.DesencriptarDatos(_servicioCorreo.srvCorreo_password);
            _port = _servicioCorreo.srvCorreo_port;    
        }

        public async Task distribuirMensajeCorreo()
        {
            //este se va a encargar de repartir los mensajes a todos los responsables del regimen... enviar los respectivos correos...
            Regimen regimen = await _regimen.obtenerUltimoRegimen();

            if (regimen != null)
            {
                ICollection<Usuario> usuarios = await _usuario.Listar();

                if ( !usuarios.IsNullOrEmpty() )
                {
                    obtenerDatosDesencriptadosCorreo();

                    foreach (Usuario usuario in usuarios)
                    {
                        enviocorreo(usuario.usuario_email!, regimen, usuario.usuario_nombre!);
                    }
                }
            }
        }

        public bool enviocorreo(string to, Regimen datos, string UsuarioFinal)
        {
            try
            {
                var mensajeEnvio = new MimeMessage();
                mensajeEnvio.From.Add(new MailboxAddress("Diaf", _from));
                mensajeEnvio.To.Add(new MailboxAddress("Usuarios Finales Regimen", to));
                mensajeEnvio.Subject = "Proceso Creación Regimen Inicialización";
                mensajeEnvio.Body = new BodyBuilder() { 
                    HtmlBody = String.Format(
$@"
<h1>Sistema Regimen</h1>
<p>Estimado {UsuarioFinal}, actualmente se ha dado inicio a un nuevo <strong>Regimen</strong>, puedes inspeccionarlo visitando tu 
panel administrativo en esta direccion ➡️ <a href='http://192.168.2.5/'>Inicio Sesion Regimen</a></p>
<hr/>
<h4>Datos Regimen</h4>
<ul>
<li>Responsable: {datos.responsableRegimen.First().usuario.usuario_nombre} </li>
<li>Objetivo: {datos.regimen_objetivo}</li>
<li>Fecha Creacion: {datos.regimen_fecha_creacion}</li>
<li>Codigo Regimen: {datos.regimen_cod}</li>
</ul>
", "")
                }.ToMessageBody();

                using ( var client = new SmtpClient() )
                {
                    try
                    {
                        client.Connect(_smtp, _port, true);
                        client.Authenticate(_from, _pass);
                        client.Send(mensajeEnvio);
                        Console.WriteLine("Correo enviado con éxito.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al enviar el correo: {ex.Message}");
                    }
                    finally
                    {
                        client.Disconnect(true);
                    }
                }

                return true;
            }
            catch( Exception e)
            {
                return false;
            }
        }
    }
}
