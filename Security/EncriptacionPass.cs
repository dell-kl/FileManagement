using Microsoft.AspNetCore.DataProtection;
using System.Drawing.Imaging;
using System.Security.Cryptography;

namespace SIS_DIAF.Security
{
    public class EncriptacionPass 
    {
        private readonly IDataProtector _protector;

        private readonly byte[] key = {
                0x00, 0x22, 0xAA, 0xEE,
                0xFF, 0x12, 0xFB, 0x99,
                0x00, 0x22, 0x00, 0xAA,
                0xFA, 0x10, 0xF2, 0x09
        };

        private readonly byte[] iv = {
                0xBB, 0x59, 0x12, 0xAA,
                0xFF, 0xFF, 0xDD, 0x02,
                0x00, 0xDD, 0xAA, 0xAA,
                0xCC, 0x23, 0xF2, 0x00
        };

        public EncriptacionPass(
            IDataProtectionProvider provider
        ) 
        {
            _protector = provider.CreateProtector("SWRlbnRpZmljYWRvciBVbmljbyBkZSBQcm90ZWNjaW9uIGRlIGxhIERJQUYK");
        }

        public string  EncriptarPassword(string passwordEntrante)
        {
            /*Código*/
            string password = _protector.Protect(passwordEntrante);
            return password;
        }

        public string EncriptarDatos(string dato)
        {
            dato = _protector.Protect(dato);
            return dato;
        }

        public string DesencriptarDatos(string encriptado)
        {
            string desencriptado = _protector.Unprotect(encriptado);
            return desencriptado;
        }

        public string EncriptrarUrl(string entradaParametro)
        {
            Aes n = Aes.Create();
            //vamos a generar una clave generar.
            
            //debemos generar un Key y IV....
            n.Key = this.key;
            n.IV = this.iv;

            //nosotros creamos nuestro encriptador...
            ICryptoTransform encriptado = n.CreateEncryptor(n.Key, n.IV);

            
            //instanciamos un espacio de memoria general
            using ( MemoryStream memoria = new MemoryStream() )
            {
                //le generamos las claves, la memoria, el tipo de encriptado.
                CryptoStream encriptacion = new CryptoStream(memoria, encriptado, CryptoStreamMode.Write);
            
                using ( StreamWriter escrituraMemoria = new StreamWriter(encriptacion) )
                {
                    escrituraMemoria.Write(entradaParametro);
                }

                //obtenemos los datos ya escritos dentro de la memoria asignada. 
                byte[] datosEscritos = memoria.ToArray();
                string c = Convert.ToBase64String(datosEscritos);
                //c = c.Replace("+", "-").Replace("/", "A").Replace("=", "0");

                return c;
            }

        }

        public string DesencriptarUrl(string entradaEncriptada)
        {
            //vamos adjuntar los caracteres que terminaron siendo reemplazados.
            //entradaEncriptada = entradaEncriptada.Replace("-", "+").Replace("A", "/").Replace("0", "=");

            Aes n = Aes.Create();
            //vamos a generar una clave generar.

            //debemos generar un Key y IV....
            n.Key = this.key;
            n.IV = this.iv;

            //nosotros creamos nuestro encriptador...
            ICryptoTransform encriptado = n.CreateDecryptor(n.Key, n.IV);

            byte[] datosHex = Convert.FromBase64String(entradaEncriptada);
            //instanciamos un espacio de escritura general
            using (MemoryStream memoria = new MemoryStream(datosHex))
            {
                //le generamos las claves, la memoria, el tipo de encriptado.
                CryptoStream encriptacion = new CryptoStream(memoria, encriptado, CryptoStreamMode.Read);

                using (StreamReader escrituraMemoria = new StreamReader(encriptacion))
                {
                    string datoFinalDesencriptado = escrituraMemoria.ReadToEnd();

                    return datoFinalDesencriptado;
                }

            }

        }

        public string DesencriptarPassword(string passwordEntranteHash)
        {
            string password = _protector.Unprotect(passwordEntranteHash);
            return password;
        }


    }
}
