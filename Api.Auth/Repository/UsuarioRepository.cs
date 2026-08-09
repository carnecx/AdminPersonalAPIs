using Api.Auth.Entities;
using Dapper;

namespace Api.Auth.Repository
{
    // repositorio encargado de consultar y actualizar usuarios
    public class UsuarioRepository
    {
        // fabrica utilizada para crear conexiones a la base de datos
        private readonly IDbConnectionFactory _connectionFactory;

        // constructor del repositorio
        public UsuarioRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // busca un usuario por su nombre de usuario
        public async Task<Usuario?> BuscarPorUsuarioAsync(
            string nombreUsuario)
        {
            // crea una conexion con mysql
            using var conexion =
                _connectionFactory.CrearConexion();

            // consulta la informacion necesaria para autenticar al usuario
            const string sql = @"
                SELECT
                    u.id_usuario AS IdUsuario,
                    u.nombre_usuario AS NombreUsuario,
                    u.nombre_completo AS NombreCompleto,
                    u.correo AS Correo,
                    u.contrasena AS Contrasena,
                    u.estado AS Estado,
                    u.intentos_fallidos AS IntentosFallidos
                FROM usuario u
                WHERE u.nombre_usuario = @nombreUsuario
                LIMIT 1;";

            // ejecuta la consulta
            return await conexion
                .QueryFirstOrDefaultAsync<Usuario>(
                    sql,
                    new
                    {
                        nombreUsuario
                    }
                );
        }

        // obtiene el rol asociado al usuario
        public async Task<string?> ObtenerRolAsync(
            int idUsuario)
        {
            // crea una conexion con mysql
            using var conexion =
                _connectionFactory.CrearConexion();

            // obtiene el nombre del rol mediante usuario_rol
            const string sql = @"
                SELECT r.nombre_rol
                FROM rol r
                INNER JOIN usuario_rol ur
                    ON ur.id_rol = r.id_rol
                WHERE ur.id_usuario = @idUsuario
                LIMIT 1;";

            // ejecuta la consulta y devuelve el rol
            return await conexion
                .QueryFirstOrDefaultAsync<string>(
                    sql,
                    new
                    {
                        idUsuario
                    }
                );
        }

        // registra un intento fallido y bloquea al usuario al tercer intento
        public async Task<int> RegistrarIntentoFallidoAsync(
            Usuario usuario)
        {
            // crea una conexion con mysql
            using var conexion =
                _connectionFactory.CrearConexion();

            // calcula la nueva cantidad de intentos
            int nuevosIntentos =
                usuario.IntentosFallidos + 1;

            // determina el nuevo estado
            string nuevoEstado =
                nuevosIntentos >= 3
                    ? "Bloqueado"
                    : usuario.Estado;

            // actualiza los intentos y el estado
            const string sql = @"
                UPDATE usuario
                SET
                    intentos_fallidos = @nuevosIntentos,
                    estado = @nuevoEstado
                WHERE id_usuario = @idUsuario;";

            // ejecuta la actualizacion
            await conexion.ExecuteAsync(
                sql,
                new
                {
                    nuevosIntentos,
                    nuevoEstado,
                    idUsuario = usuario.IdUsuario
                }
            );

            // devuelve la nueva cantidad de intentos
            return nuevosIntentos;
        }

        // reinicia los intentos fallidos despues de un login correcto
        public async Task ReiniciarIntentosAsync(
            int idUsuario)
        {
            // crea una conexion con mysql
            using var conexion =
                _connectionFactory.CrearConexion();

            // coloca los intentos nuevamente en cero
            const string sql = @"
                UPDATE usuario
                SET intentos_fallidos = 0
                WHERE id_usuario = @idUsuario;";

            // ejecuta la actualizacion
            await conexion.ExecuteAsync(
                sql,
                new
                {
                    idUsuario
                }
            );
        }
    }
}