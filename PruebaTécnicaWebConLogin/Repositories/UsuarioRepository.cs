using Dapper;
using Microsoft.Data.SqlClient;
using PruebaTécnicaWebConLogin.Models;
using System.Data;

namespace PruebaTécnicaWebConLogin.Repositories
{
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Usuario ObtenerPorUsername(string username)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                // Agregada FechaBloqueo a la consulta
                const string query = @"SELECT 
                                        Id, 
                                        Username, 
                                        PasswordHash, 
                                        Email, 
                                        IntentosFallidos, 
                                        Bloqueado, 
                                        RoleId,
                                        FechaBloqueo 
                                       FROM dbo.Usuarios 
                                       WHERE Username = @Username";

                return connection.QueryFirstOrDefault<Usuario>(query, new { Username = username });
            }
        }

        public void IncrementarIntentosFallidos(string username)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                const string query = @"UPDATE dbo.Usuarios 
                                       SET IntentosFallidos = ISNULL(IntentosFallidos, 0) + 1 
                                       WHERE Username = @Username";

                connection.Execute(query, new { Username = username });
            }
        }

        public void BloquearCuenta(string username)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                // Setea Bloqueado en 1 y la estampa de tiempo actual del servidor
                const string query = @"UPDATE dbo.Usuarios 
                                       SET Bloqueado = 1,
                                           FechaBloqueo = GETDATE() 
                                       WHERE Username = @Username";

                connection.Execute(query, new { Username = username });
            }
        }

        public void RestablecerIntentos(string username)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                // Limpia tanto los intentos como la fecha de bloqueo
                const string query = @"UPDATE dbo.Usuarios 
                                       SET IntentosFallidos = 0,
                                           Bloqueado = 0,
                                           FechaBloqueo = NULL 
                                       WHERE Username = @Username";

                connection.Execute(query, new { Username = username });
            }
        }
    }
}
