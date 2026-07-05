using System.Security.Cryptography;
using System.Text;

namespace PruebaTécnicaWebConLogin.Services
{
    public class PasswordHasher
    {
        /// <summary>
        /// Genera el hash SHA256 en formato hexadecimal a partir de un texto plano.
        /// </summary>
        public string HashPassword(string passwordPlano)
        {
            if (string.IsNullOrWhiteSpace(passwordPlano))
                return string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordPlano));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        /// <summary>
        /// Convierte el texto plano en un hash SHA256 para compararlo con la Base de Datos.
        /// </summary>
        public bool VerificarPassword(string passwordPlano, string hashBaseDatos)
        {
            if (string.IsNullOrWhiteSpace(passwordPlano) || string.IsNullOrWhiteSpace(hashBaseDatos))
                return false;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwordPlano));
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // Convierte a cadena hexadecimal
                }

                // Compara el hash generado contra el almacenado (ignorando mayúsculas/minúsculas)
                return builder.ToString().Equals(hashBaseDatos, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
