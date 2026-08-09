using System.Data;
using MySqlConnector;

namespace Api.Auth.Repository
{
    // crea las conexiones con la base de datos
    public class DbConnectionFactory : IDbConnectionFactory
    {
        // guarda la cadena de conexion
        private readonly string _cadenaConexion;

        // recibe la configuracion mediante inyeccion de dependencias
        public DbConnectionFactory(IConfiguration configuration)
        {
            // obtiene la cadena de conexion del appsettings.json
            _cadenaConexion =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se encontro la cadena de conexion."
                );
        }

        // crea una nueva conexion con mysql
        public IDbConnection CrearConexion()
        {
            return new MySqlConnection(_cadenaConexion);
        }
    }
}