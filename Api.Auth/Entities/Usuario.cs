namespace Api.Auth.Entities
{
    // representa un usuario de la base de datos
    public class Usuario
    {
        // identificador del usuario
        public int IdUsuario { get; set; }

        // nombre utilizado para iniciar sesion
        public string NombreUsuario { get; set; } = string.Empty;

        // nombre completo del usuario
        public string NombreCompleto { get; set; } = string.Empty;

        // correo del usuario
        public string Correo { get; set; } = string.Empty;

        // contrasena almacenada en la base de datos
        public string Contrasena { get; set; } = string.Empty;

        // estado actual del usuario
        public string Estado { get; set; } = string.Empty;

        // cantidad de intentos fallidos
        public int IntentosFallidos { get; set; }

        // rol asignado al usuario
        public string Rol { get; set; } = string.Empty;
    }
}