using Dapper;

namespace Api.Auth.Repository
{
    // repositorio encargado de guardar registros en la bitacora
    public class BitacoraRepository
    {
        // fabrica utilizada para crear conexiones a la base de datos
        private readonly IDbConnectionFactory _connectionFactory;

        // constructor del repositorio
        public BitacoraRepository(
            IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // registra una accion realizada por un usuario
        public async Task RegistrarAsync(
            int idUsuario,
            string descripcion)
        {
            // crea una conexion con mysql
            using var conexion =
                _connectionFactory.CrearConexion();

            // inserta la accion en la tabla bitacora
            const string sql = @"
                INSERT INTO bitacora
                (
                    id_usuario,
                    descripcion
                )
                VALUES
                (
                    @idUsuario,
                    @descripcion
                );";

            // ejecuta el insert
            await conexion.ExecuteAsync(
                sql,
                new
                {
                    idUsuario,
                    descripcion
                }
            );
        }
    }
}