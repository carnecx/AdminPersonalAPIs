using System.Data;

namespace Api.Auth.Repository
{
    // define el metodo para crear conexiones con la base de datos
    public interface IDbConnectionFactory
    {
        // crea y devuelve una nueva conexion
        IDbConnection CrearConexion();
    }
}