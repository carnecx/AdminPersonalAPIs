using System.Security.Cryptography;
using System.Text;

namespace Api.Auth.Security
{
    // servicio encargado de encriptar, desencriptar y validar contrasenas
    public class PasswordService
    {
        // clave utilizada por el algoritmo aes gcm
        // debe tener 32 caracteres para trabajar con aes 256
        private readonly string clave =
            "12345678901234567890123456789012";

        // encripta una contrasena antes de almacenarla en la base de datos
        public string Encriptar(string texto)
        {
            // convierte la clave a bytes
            byte[] key =
                Encoding.UTF8.GetBytes(clave);

            // genera un nonce aleatorio
            byte[] nonce =
                RandomNumberGenerator.GetBytes(12);

            // convierte la contrasena a bytes
            byte[] textoBytes =
                Encoding.UTF8.GetBytes(texto);

            // arreglo donde se almacenara el texto cifrado
            byte[] cifrado =
                new byte[textoBytes.Length];

            // etiqueta de autenticacion
            byte[] tag =
                new byte[16];

            // crea una instancia de aes gcm
            using var aes =
                new AesGcm(key, 16);

            // realiza el cifrado
            aes.Encrypt(
                nonce,
                textoBytes,
                cifrado,
                tag
            );

            // devuelve nonce, cifrado y tag
            // separados por puntos
            return
                Convert.ToBase64String(nonce)
                + "."
                + Convert.ToBase64String(cifrado)
                + "."
                + Convert.ToBase64String(tag);
        }

        // desencripta una contrasena almacenada en la base de datos
        public string Desencriptar(
            string textoEncriptado)
        {
            // convierte la clave a bytes
            byte[] key =
                Encoding.UTF8.GetBytes(clave);

            // separa las tres partes almacenadas
            string[] partes =
                textoEncriptado.Split('.');

            // valida que tenga nonce, cifrado y tag
            if (partes.Length != 3)
            {
                throw new FormatException(
                    "El formato de la contrasena encriptada no es valido."
                );
            }

            // obtiene el nonce
            byte[] nonce =
                Convert.FromBase64String(
                    partes[0]
                );

            // obtiene el texto cifrado
            byte[] cifrado =
                Convert.FromBase64String(
                    partes[1]
                );

            // obtiene el tag
            byte[] tag =
                Convert.FromBase64String(
                    partes[2]
                );

            // arreglo donde quedara la contrasena original
            byte[] texto =
                new byte[cifrado.Length];

            // crea una instancia de aes gcm
            using var aes =
                new AesGcm(key, 16);

            // realiza el descifrado
            aes.Decrypt(
                nonce,
                cifrado,
                tag,
                texto
            );

            // convierte los bytes nuevamente a texto
            return Encoding.UTF8.GetString(
                texto
            );
        }

        // valida que la contrasena digitada coincida con la almacenada
        public bool Validar(
            string contrasenaDigitada,
            string contrasenaBD)
        {
            try
            {
                // desencripta la contrasena almacenada
                string contrasenaOriginal =
                    Desencriptar(
                        contrasenaBD
                    );

                // compara ambas contrasenas
                return contrasenaDigitada ==
                       contrasenaOriginal;
            }
            catch
            {
                // si ocurre un error devuelve false
                return false;
            }
        }
    }
}